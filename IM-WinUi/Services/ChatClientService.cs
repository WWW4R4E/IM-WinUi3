using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
            Debug.WriteLine("开始初始化 Chat HubConnection...");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5287/ChatHub", options =>
                {
                    options.Headers.Add("Authorization", $"Bearer {_jwtToken}");
                })
                .Build();

            _hubConnection.On<string>("ReceiveMessage", ReceiveMessage);
            _hubConnection.On<Task>("SendMessageFailed", SendMessageFailed);

            try
            {
                await _hubConnection.StartAsync();

                // 检查连接状态以确认是否成功连接
                Debug.WriteLine(_hubConnection.State == HubConnectionState.Connected
                    ? "HubConnection 已成功启动。"
                    : $"HubConnection 启动失败，当前状态: {_hubConnection.State}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"启动 HubConnection 失败: {ex.Message}");
                // TODO 实现服务器重连
                //await ShowDiaglog.ShowMessage("提示", "连接服务器失败，请检查网络连接。");
                //while (_hubConnection.State != HubConnectionState.Connected)
                //{
                //    try
                //    {
                //        await _hubConnection.StartAsync();
                //    }
                //    catch (Exception e)
                //    {
                //        Debug.WriteLine($"重试连接失败: {e.Message}");
                //    }
                //}
            }
        }


        private void ReceiveMessage(string messageJson)
        {
            var message = JsonSerializer.Deserialize<IMMessage>(messageJson);

            if (message == null)
            {
                Debug.WriteLine("反序列化后的消息为 null。");
                throw new ArgumentNullException(nameof(message), "反序列化后的消息为 null。");
            }

            var content = Ioc.Default.GetRequiredService<LocalDbcontext>();
            content.CreateMessage(message);

            Debug.WriteLine("消息已保存到数据库。");
            OnMessageReceive(new MessageReceiveEventArgs { Success = true });

        }

        protected virtual void OnMessageReceive(MessageReceiveEventArgs e)
        {
            Debug.WriteLine(1);
            MessageSent(this, e);
        }

        private Task SendMessageFailed()
        {
            Debug.WriteLine("发送失败了");
            return Task.CompletedTask;
        }
        public async Task<bool> SendMessageAsync(IMMessage message)
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                Debug.WriteLine("连接未激活，无法发送消息");
                throw new InvalidOperationException("连接未激活，无法发送消息");
            }

            try
            {
                await _hubConnection.InvokeAsync("SendMessage", message);
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