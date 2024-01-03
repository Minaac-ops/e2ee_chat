using e2ee_chat.Core.Models;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessagePublisher
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

    public void PublishMessage(string receiver,string publisher, byte[] encryptedMsg)
    {
        var message = new Message
        {
            Publisher = publisher,
            Receiver = receiver,
            EncryptedMessage = encryptedMsg
        };
        // publish message with routing key as username
        _bus.PubSub.Publish(message,$"user.{receiver}");
    }

    public void PublishPublicKey(byte[] publicKey, string publisher, string receiver)
    {
        try
        {
            var message = new PublicKeyMessage
            {
                PublicKey = publicKey,
                Publisher = publisher,
                Receiver = receiver
            };
            _bus.PubSub.Publish(message, $"{receiver}");
            Console.WriteLine($"Message request send to {message.Receiver}. You will be notified if they accept.");
            Thread.Sleep(3000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Contact support.");
        }
    }
}