using IMWinUi.Models;
using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace IMWinUi.Services
{
    internal class SignalRService
    {
        private HubConnection _hubConnection;
        private IConnection _rabbitMqConnection;
        private IChannel _rabbitMqChannel;
        private readonly Dictionary<string, Action<IMMessage>> _messageHandlers = new();

        public event Action<string> OnConnectionStatusChanged;

        public async Task ConnectAsync(string serverUrl, string userId)
        {
            try
            {
                // 连接 SignalR
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{serverUrl}/chatHub", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult("your-access-token"); // 如果需要身份验证
                    })
                    .Build();

                // 注册接收实时消息的回调
                _hubConnection.On<IMMessage>("ReceiveMessage", message =>
                {
                    if (_messageHandlers.TryGetValue(message.ReceiverName, out var handler))
                    {
                        handler(message);
                    }
                });

                _hubConnection.Closed += async (error) =>
                {
                    OnConnectionStatusChanged?.Invoke("Disconnected. Reconnecting...");
                    await Task.Delay(5000);
                    await ConnectAsync(serverUrl, userId);
                };

                await _hubConnection.StartAsync();
                OnConnectionStatusChanged?.Invoke("Connected to SignalR");

                // 连接 RabbitMQ
                var factory = new ConnectionFactory() { HostName = "localhost" }; // 替换为实际 RabbitMQ 地址
                _rabbitMqConnection = await factory.CreateConnectionAsync();
                _rabbitMqChannel = await _rabbitMqConnection.CreateChannelAsync();
                _rabbitMqChannel.QueueDeclareAsync(queue: $"user_{userId}", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_rabbitMqChannel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<IMMessage>(messageJson);

                    if (message != null && _messageHandlers.TryGetValue(message.ReceiverName, out var handler))
                    {
                        handler(message);
                    }
                };

                _rabbitMqChannel.BasicConsumeAsync(queue: $"user_{userId}", autoAck: true, consumer: consumer);
                OnConnectionStatusChanged?.Invoke("Connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke($"Connection failed: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(IMMessage message)
        {
            try
            {
                await _hubConnection.SendAsync("SendMessage", message);
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke($"Send message failed: {ex.Message}");
            }
        }

        public void SubscribeToMessages(string receiverName, Action<IMMessage> handler)
        {
            _messageHandlers[receiverName] = handler;
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                _hubConnection.DisposeAsync();
            }

            if (_rabbitMqChannel != null)
            {
                _rabbitMqChannel.CloseAsync();
                _rabbitMqChannel.Dispose();
            }

            if (_rabbitMqConnection != null)
            {
                _rabbitMqConnection.CloseAsync();
                _rabbitMqConnection.Dispose();
            }

            OnConnectionStatusChanged?.Invoke("Disconnected");
        }
    }
}
