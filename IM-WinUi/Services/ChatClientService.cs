using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace IMWinUi.Services
{
    internal class ChatClientService
    {
        private HubConnection _hubConnection;
        private readonly string _jwtToken;
        public event EventHandler<MessageReceiveEventArgs> MessageSent;

        public ChatClientService()
        {
            _jwtToken = Properties.Settings.Default.JwtToken;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            Debug.WriteLine("开始初始化聊天");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5287/ChatHub", options =>
                {
                    options.Headers.Add("Authorization", $"Bearer {_jwtToken}");
                })
                .Build();

            // 添加Hub连接关闭事件监听
            _hubConnection.Closed += async (error) => await StartConnectionAsync();

            _hubConnection.On<string>("ReceiveMessage", ReceiveMessage);
            _hubConnection.On<Task>("SendMessageFailed", SendMessageFailed);

            try
            {
                await _hubConnection.StartAsync();

                Debug.WriteLine(_hubConnection.State == HubConnectionState.Connected
                    ? "HubConnection 已成功启动。"
                    : $"HubConnection 启动失败，当前状态: {_hubConnection.State}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"启动 HubConnection 失败: {ex.Message}");
                // 调用重连逻辑
                await StartConnectionAsync();
            }
        }

        // 新增重连方法
        private async Task StartConnectionAsync()
        {
            int retryCount = 0;
            const int maxRetryCount = 5;
            const int initialDelay = 2000; // 初始延迟2秒
            int retryDelay = initialDelay;

            while (_hubConnection?.State != HubConnectionState.Connected && retryCount < maxRetryCount)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    retryCount = 0; // 重置计数器
                    Debug.WriteLine("重新连接成功");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    retryDelay = initialDelay * (int)Math.Pow(2, retryCount); // 指数退避
                    Debug.WriteLine($"重连失败，第{retryCount}次尝试，将在{retryDelay}毫秒后重试: {ex.Message}");
                    await Task.Delay(retryDelay);
                }
            }
        }

        private void ReceiveMessage(string messageJson)
        {
            var message = JsonSerializer.Deserialize<LocalMessage>(messageJson);

            if (message == null)
            {
                Debug.WriteLine("反序列化后的消息为 null。");
                throw new ArgumentNullException(nameof(message), "反序列化后的消息为 null。");
            }

            var content = Ioc.Default.GetRequiredService<LocalDbContext>();
            content.AddMessages(new List<LocalMessage>{message});

            Debug.WriteLine("消息已保存到数据库。");
            OnMessageReceive(new MessageReceiveEventArgs { Success = true });

        }

        protected virtual void OnMessageReceive(MessageReceiveEventArgs e)
        {
            MessageSent(this, e);
        }

        private Task SendMessageFailed()
        {
            Debug.WriteLine("发送失败了");
            return Task.CompletedTask;
        }
        public async Task<bool> SendMessageAsync(LocalMessage message)
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                Debug.WriteLine("连接未激活，无法发送消息");
                throw new InvalidOperationException("连接未激活，无法发送消息");
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(message);
                await _hubConnection.InvokeAsync("SendPrivateMessage", messageJson);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发送消息失败: {ex.Message}");
                throw;
            }
        }
    }
    // 定义事件参数类
    public class MessageReceiveEventArgs : EventArgs
    {
        public bool Success { get; init; }
    }
}