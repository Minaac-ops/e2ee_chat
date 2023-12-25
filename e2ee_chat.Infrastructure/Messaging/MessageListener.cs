using e2ee_chat.Core.Interfaces.Messaging;
using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Schemas;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessageListener : IMessageListener
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

    public async Task Start()
    {
        while (true)
        {
            _bus = RabbitHutch.CreateBus(_config.GetConnectionString("RabbitMQ") ?? throw  new InvalidOperationException("Connection string is null"));
        
            await _bus.PubSub.SubscribeAsync<Message>($"user.{_loggedInUser}" ,message => HandleMessageReceived(message), x => x.WithTopic($"user.{_loggedInUser}"));

            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }

    private void HandleMessageReceived(Message msg)
    {
        Console.WriteLine($"{msg.Publisher}: {msg.PlainTextMesasge}");
    }

    public Task Listen()
    {
        throw new NotImplementedException();
    }
}