using ChatRoomASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ChatRoomASP.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatHub> _logger;
        // private readonly IRabbitMqClient _rabbitMqClient;

        // public ChatHub(AppDbContext context, IRabbitMqClient rabbitMqClient, ILogger<ChatHub> logger)
        // {
        //     _context = context;
        //     _rabbitMqClient = rabbitMqClient;
        //     _logger = logger;
        // }
        public ChatHub(AppDbContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 加入群组
        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        // 发送群聊消息
        public async Task SendGroupMessage(string groupId, string message)
        {
            await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", Context.UserIdentifier, message);
        }

        // 发送私聊消息
        public async Task SendPrivateMessage(IMMessage message)
        {
            // 存储消息到数据库
            await _context.IMMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            // 将消息序列化为 JSON 字符串
            var messageJson = JsonSerializer.Serialize(message);

            // 通知发送者服务器已接收消息
            await Clients.Caller.SendAsync("ReceiveMessage", messageJson);

            try
            {
                // 尝试通过 SignalR 直接发送消息
                await Clients.User(message.ReceiverName).SendAsync("ReceiveMessage", messageJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("用户 {ReceiverName} 离线，无法发送消息：{Message}", message.ReceiverName, ex.Message);

                // 如果接收者离线，将消息发布到消息队列（预留功能）
                // var messageBytes = Encoding.UTF8.GetBytes(messageJson);
                // await _rabbitMqClient.PublishAsync("im_messages", messageBytes);
                // await Clients.Caller.SendAsync("SendMessageQueued", "Message queued for delivery.");
            }
        }

        // 检查用户是否在线（预留功能）
        // private async Task<bool> IsUserOnline(string userId)
        // {
        //     var user = await _userManager.FindByIdAsync(userId);
        //     if (user != null && _context.UserConnections.TryGetValue(user.UserId, out _))
        //     {
        //         return true;
        //     }
        //     return false;
        // }

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