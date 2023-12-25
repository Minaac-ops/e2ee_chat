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

    public void PublishMessage(string username, string plaintextMsg)
    {
        Console.WriteLine("Publish Message ");
        var message = new Message
        {
            Username = username,
            PlainTextMesasge = plaintextMsg
        };
        _bus.PubSub.Publish(message, "newMessage");
        Console.WriteLine("Message published with username " + username+ " and msg: " + plaintextMsg);
    }

}