using ChatRoomASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ChatRoomASP.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChatHub> _logger;
        // private readonly IRabbitMqClient _rabbitMqClient;

        // public ChatHub(AppDbContext context, UserManager<ApplicationUser> userManager, IRabbitMqClient rabbitMqClient, ILogger<ChatHub> logger)
        // {
        //     _context = context;
        //     _userManager = userManager;
        //     _rabbitMqClient = rabbitMqClient;
        //     _logger = logger;
        // }
        public ChatHub(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<ChatHub> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // TODO 客户端连接到 SignalR 集线器（Hub）时被调用,用于根据 Context.User 获取对应的 ApplicationUser 对象。

        public override async Task OnConnectedAsync()
        {       
            // 获取当前连接的用户信息
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                // 将用户的 ID 和连接 ID 存储在字典中
                _context.UserConnections[user.Id] = Context.ConnectionId;
                // 调用基类的 OnConnectedAsync 方法
                await base.OnConnectedAsync();
            }
        }
        // TODO 客户端断开连接时被调用,用于根据 Context.User 获取对应的 ApplicationUser 对象。
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // 获取当前连接的用户信息
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                // 从字典中移除用户的 ID 和连接 ID
                _context.UserConnections.TryRemove(user.Id, out _);
                // 调用基类的 OnDisconnectedAsync 方法
                await base.OnDisconnectedAsync(exception);
            }
        }

        public async Task SendMessage(IMMessage message)
        {
            // 存储消息到数据库
            await _context.IMMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            // 将消息序列化为 JSON 字符串
            var messageJson = JsonSerializer.Serialize(message);
            // 通知发送者服务器已接收消息
            await Clients.All.SendAsync("ReceiveMessage", messageJson);
            // 获取接收者的用户ID
            var receiver = await _userManager.FindByNameAsync(message.ReceiverName);

            // 尝试通过 SignalR 发送消息
            if (_context.UserConnections.TryGetValue(receiver.Id, out var receiverConnectionId))
            {
                _logger.LogInformation("尝试发送消息");
                // 向接收者发送消息
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", messageJson);  
                // await Clients.Caller.SendAsync("SendMessageSuccess", message);
            }
            // else
            // {
            //     // 如果接收者不在线，将消息发布到 RabbitMQ 队列
            //     var messageJson = JsonSerializer.Serialize(message);
            //     var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            //     // await _rabbitMqClient.PublishAsync("im_messages", messageBytes);
            //     await Clients.Caller.SendAsync("SendMessageQueued", "Message queued for delivery.");
            // }

        }

        // public async Task SubscribeToMessages()
        // {
        //     // 订阅 RabbitMQ 队列
        //     await _rabbitMqClient.SubscribeAsync("im_messages", async (model, ea) =>
        //     {
        //         var body = ea.Body.ToArray();
        //         var messageJson = Encoding.UTF8.GetString(body);
        //         var message = JsonSerializer.Deserialize<IMMessage>(messageJson);

        //         if (message != null)
        //         {
        //             // 获取接收者的用户ID
        //             var receiver = await _userManager.FindByNameAsync(message.ReceiverName);
        //             if (receiver != null && _context.UserConnections.TryGetValue(receiver.Id, out var receiverConnectionId))
        //             {
        //                 // 向接收者发送消息
        //                 await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
        //             }
        //         }
        //     });
        // }
    }
}