using e2ee_chat.Core.Models;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessageListener
{
    IBus _bus;
    private readonly IConfiguration _config;
    private readonly UserModel _loggedInUser;

    public MessageListener(UserModel loggedInUser)
    {
        _loggedInUser = loggedInUser;
        var appSettings = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "e2ee_chat.Infrastructure", "appsettings.json");
        _config = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
    }

    public void Start()
    {
        _bus = RabbitHutch.CreateBus(_config.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("Connection string is null"));
        _bus.PubSub.Subscribe<Message>("Message", HandleMessageReceived,x => x.WithTopic("newMessage"));

        lock (this)
        {
            Monitor.Wait(this);
        }
    }

    private void HandleMessageReceived(Message msg)
    {
        Console.WriteLine(msg.PlainTextMesasge);
        Console.WriteLine("Message: "+msg.PlainTextMesasge+" from: " + msg.Username);
    }
}