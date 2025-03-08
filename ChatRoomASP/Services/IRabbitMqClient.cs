using System;
using RabbitMQ.Client.Events;

namespace ChatRoomASP.Models;

public interface IRabbitMqClient
{
    Task PublishAsync(string queueName, byte[] message); 
    Task SubscribeAsync(string queueName, EventHandler<BasicDeliverEventArgs> onMessageReceived); 
}