using System.Security.Cryptography;
using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Messaging;
using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Schemas;
using e2ee_chat.Util;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace e2ee_chat.Infrastructure.Messaging;

public class MessageListener : IMessageListener
{
    IBus _bus;
    private readonly IConfiguration _config;
    private readonly UserModel _loggedInUser;
    private byte[] _sharedSecret;
    private readonly IAuthUtil _authUtil;

    public MessageListener(UserModel loggedInUser, IAuthUtil authUtil)
    {
        _authUtil = authUtil;
        _loggedInUser = loggedInUser;
        var appSettings = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "e2ee_chat.Infrastructure", "appsettings.json");
        _config = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
    }

    public async Task Start()
    {
        Console.WriteLine("listening for: "+_loggedInUser.Username);
        _bus = RabbitHutch.CreateBus(_config.GetConnectionString("RabbitMQ") ?? throw  new InvalidOperationException("Connection string is null"));

        while (true)
        {
            await _bus.PubSub.SubscribeAsync<Message>(_loggedInUser.Username ,message => HandleMessageReceived(message), x => x.WithTopic($"user.{_loggedInUser.Username}"));
            await _bus.PubSub.SubscribeAsync<PublicKeyMessage>($"{_loggedInUser.Username}",
                message => HandlePublicKeyReceived(message), x => x.WithTopic($"{_loggedInUser.Username}"));
            
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }

    private void HandlePublicKeyReceived(PublicKeyMessage message)
    {
        Console.WriteLine($"Public key received from {message.Publisher}");
        // using (var dh = new ECDiffieHellmanCng())
        // {
        //     dh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        //     dh.HashAlgorithm = CngAlgorithm.Sha512;
        //     _loggedInUser.PublicKey = dh.PublicKey.ToByteArray();
        //     _sharedSecret = dh.DeriveKeyMaterial(CngKey.Import(message.PublicKey, CngKeyBlobFormat.EccPublicBlob));
        // }
        //_authUtil.GenerateSharedSecret(message.PublicKey, message.Receiver);
        var instance = Crypto.Instance;
        instance.GenerateSharedSecret(message.PublicKey);
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