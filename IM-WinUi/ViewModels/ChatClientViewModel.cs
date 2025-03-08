using IMWinUi.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace IMWinUi.ViewModels
{
    internal class ChatClientViewModel
    {
        private HubConnection _hubConnection;
        private readonly string _jwtToken;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        public ChatClientViewModel()
        {
            _jwtToken = Properties.Settings.Default.JwtToken;
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("开始初始化 HubConnection...");

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
                Debug.WriteLine("尝试启动 HubConnection...");
                await _hubConnection.StartAsync();

                // 检查连接状态以确认是否成功连接
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    Debug.WriteLine("HubConnection 已成功启动。");
                }
                else
                {
                    Debug.WriteLine($"HubConnection 启动失败，当前状态: {_hubConnection.State}");
                }
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
            try
            {
                var message = JsonSerializer.Deserialize<IMMessage>(messageJson);

                using (LocalDbcontext chatingContext = new LocalDbcontext())
                {
                    chatingContext.CreateMessage(message);
                    Debug.WriteLine("消息已保存到数据库。");
                    OnMessageSent(new MessageSentEventArgs { Success = true});
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"处理消息失败: {ex.Message}");
            }
        }
        protected virtual void OnMessageSent(MessageSentEventArgs e)
        {
            MessageSent?.Invoke(this, e);
        }

        public async Task SendMessageFailed()
        {
            Debug.WriteLine("发送失败了");
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
    public class MessageSentEventArgs : EventArgs
    {
        public bool Success { get; set; }
    }
}