using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomASP.Models
{
    public class RabbitMqClient : IRabbitMqClient, IDisposable
    {
        private readonly RabbitMqOptions _options;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMqClient(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            InitializeConnectionAndChannel().GetAwaiter().GetResult();
        }

        private async Task InitializeConnectionAndChannel()
        {
            _connection = await CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task PublishAsync(string queueName, byte[] message)
        {
            await _channel.QueueDeclareAsync(queueName, true, false, false, null);

            var properties = new BasicProperties();
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: message.AsMemory(),
                cancellationToken: CancellationToken.None
            );
        }

        public async Task SubscribeAsync(string queueName, EventHandler<BasicDeliverEventArgs> onMessageReceived)
        {
            await _channel.QueueDeclareAsync(queueName, true, false, false, null);
            var consumer = new AsyncEventingBasicConsumer(_channel); // 修改: EventingBasicConsumer 改为 AsyncEventingBasicConsumer
            consumer.ReceivedAsync += async (model, ea) => onMessageReceived(model, ea); // 使用 Received 事件
            await _channel.BasicConsumeAsync(queueName, true, consumer);
        }

        private async Task<IConnection> CreateConnectionAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port
            };
            return await factory.CreateConnectionAsync();
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
