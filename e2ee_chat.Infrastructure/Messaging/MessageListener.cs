using e2ee_chat.Core.Models;
using e2ee_chat.Util;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessageListener
{
    IBus _bus;
    private readonly IConfiguration _config;
    private readonly UserModel _loggedInUser;
    private byte[] _sharedSecret;

    public MessageListener(UserModel loggedInUser)
    {
        _loggedInUser = loggedInUser;
        var appSettings = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "e2ee_chat.Infrastructure", "appsettings.json");
        _config = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
        _bus = RabbitHutch.CreateBus(_config.GetConnectionString("RabbitMQ"));
    }

    public async Task ListenForMessageRequest()
    {
                await _bus.PubSub.SubscribeAsync<PublicKeyMessage>($"{_loggedInUser.Email}",
                    message => HandlePublicKeyReceived(message), x => x.WithTopic($"{_loggedInUser.Email}"));
                await _bus.PubSub.SubscribeAsync<RequestAcceptedMessage>($"{_loggedInUser.Email}",
                    message => HandleRequestAccepted(message), x => x.WithTopic($"requestAccepted.{_loggedInUser.Email}"));
    }

    public async Task Start()
    {
        await _bus.PubSub.SubscribeAsync<Message>(_loggedInUser.Username ,message => HandleMessageReceived(message), x => x.WithTopic($"user.{_loggedInUser.Email}"));
    }
    
    private void HandleRequestAccepted(RequestAcceptedMessage message)
    {
        var instance = Crypto.Instance;
        instance.GenerateSharedSecret(message.PublicKey);
        Console.WriteLine($"{message.Publisher} accepted your message request. Go to 'Messages' to chat.");
        Thread.Sleep(2000);
    }

    private void HandlePublicKeyReceived(PublicKeyMessage message)
    {
        try
        {
            Console.WriteLine($"You received a message request from {message.Publisher}.");
            
            var instance = Crypto.Instance;
            var publicKey = instance.GetPublicKey();
            instance.GenerateSharedSecret(message.PublicKey);
            var answerMessage = new RequestAcceptedMessage
                { PublicKey = publicKey, Publisher = _loggedInUser.Username, Receiver = message.Publisher };
            _bus.PubSub.Publish(answerMessage, $"requestAccepted.{message.Publisher}");
            
            Console.WriteLine($"You can go to 'Messages' to chat.");
            Thread.Sleep(4000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Contact support.");
        }
    }

    private void HandleMessageReceived(Message msg)
    {
        try
        {
            var instance = Crypto.Instance;
            var decryptedMsg = instance.Decrypt(msg.EncryptedMessage);
            Console.WriteLine(decryptedMsg.ToLower().Equals("exit")
                ? $"{msg.Publisher} left the chat."
                : $"{msg.Publisher}: {decryptedMsg}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Contact support.");
        }
    }
}