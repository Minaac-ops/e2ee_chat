using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Messaging;
using e2ee_chat.Util;

namespace e2ee_chat;

public class E2EChatManager
{
    private readonly IAuthUtil _authUtil;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private static UserModel? _loggedInUser;

    public E2EChatManager(IAuthUtil authUtil, IUserService userService, IAuthService authService)
    {
        _authUtil = authUtil;
        _userService = userService;
        _authService = authService;
        Welcome();
    }

    private void Welcome()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Your options: \n1. Login | 2. Create new user");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    Register();
                    break;
                default:
                    Thread.Sleep(1000);
                    continue;
            }

            break;
        }
    }

    private async void Login()
    {
        Console.Clear();
        Console.WriteLine("You are about to log in. Please enter your...");
        Console.WriteLine("Email: ");
        var email = Console.ReadLine();
        Console.WriteLine("Password: ");
        var pass = Console.ReadLine();

        try
        {
            var user = _authService.Login(email, pass);
            _loggedInUser = user;
            Console.Clear();
            await Menu();
        }
        catch (Exception e)
        {
            Thread.Sleep(5000);
            Welcome();
        }
    }

    private void Register()
    {
        Console.Clear();
        Console.WriteLine("You are setting up your new account. Please write your...");
        Console.WriteLine("Email: ");
        var email = Console.ReadLine();
        Console.WriteLine("Username: ");
        var username = Console.ReadLine();
        Console.WriteLine("Password: ");
        var password = Console.ReadLine();

        try
        {
            var newUser = new UserModel
            {
                Email = email!,
                Username = username!,
                Password = password!
            };
            if (newUser.IsPasswordValid())
            {
                var user = _authUtil.PasswordHasher(email!, username!, password!);

                _userService.CreateUser(user);
                Thread.Sleep(3000);
                Welcome();
            }
            else
            {
                Console.WriteLine(
                    "Password must be at least 8 characters and must contain small and capital letters, digits and at least one special character.");
                Thread.Sleep(5000);
                Register();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Password must be at least 8 characters and must contain small and capital letters, digits and at least one special character.");
        }
    }


    private async Task Menu()
    {
        try
        {
            Console.WriteLine($"Welcome {_loggedInUser.Username}");
            while (true)
            {
                Console.WriteLine("Your options: \n1. Messages | 2. Send message request");
                var listener = new MessageListener(_loggedInUser);
                Task.Run(() => listener.ListenForMessageRequest());
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Console.Clear();
                        await Messaging();
                        break;
                    case "2":
                        Console.Clear();
                        MessageRequest();
                        break;
                    default:
                        Thread.Sleep(1000);
                        continue;
                }
                break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private async Task Messaging()
    {
        Console.WriteLine("Email of the person you want to chat with: ");
        var receiver = Console.ReadLine();
        try
        {
            var _instance = Crypto.Instance;
            Console.WriteLine("Wait...");
            Thread.Sleep(4000);
            Console.Clear();
            Console.WriteLine($"Chat with {receiver}");
            Console.WriteLine("This chat is end-2-end encrypted.");

            var listener = new MessageListener(_loggedInUser);
            Task.Run(() => listener.Start());
            var publisher = new MessagePublisher();
            Thread.Sleep(2000);

            while (true)
            {
                var msg = Console.ReadLine();
                publisher.PublishMessage(receiver, _loggedInUser.Email, _instance.Encrypt(msg));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{receiver} can't chat right now.");
        }
    }

    private async void MessageRequest()
    {
        Console.WriteLine("Write the email of the person you want to send a request to");
        var receiver = Console.ReadLine();
        var messagePublisher = new MessagePublisher();
        var instance = Crypto.Instance;
        messagePublisher.PublishPublicKey(instance.GetPublicKey(), _loggedInUser.Email, receiver);
        Thread.Sleep(2000);
        Console.Clear();
        await Menu();
    }
}