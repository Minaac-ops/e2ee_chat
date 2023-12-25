using e2ee_chat.Core.Interfaces.Messaging;
using e2ee_chat.Core.Models;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private IBus _bus;
    private readonly IConfiguration _config;
    public MessagePublisher()
    {
        var appSettings = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "e2ee_chat.Infrastructure", "appsettings.json");
        _config = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
        _bus = RabbitHutch.CreateBus(_config.GetConnectionString("RabbitMQ"));
    }

    public void PublishMessage(string receiver,string publisher, string plaintextMsg)
    {
        var message = new Message
        {
            Publisher = publisher,
            Receiver = receiver,
            PlainTextMesasge = plaintextMsg
        };
        // Declaring exchange
        _bus.Advanced.ExchangeDeclare("user_exchange", ExchangeType.Topic);
        // publish message with routing key as username
        _bus.PubSub.Publish(message,$"user.{receiver}");
    }

}