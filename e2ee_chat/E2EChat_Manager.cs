using System.ComponentModel.Design;
using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Messaging;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Models;
using e2ee_chat.Infrastructure.Messaging;

namespace e2ee_chat;

public class E2EChatManager
{
    private readonly IAuthUtil _authUtil;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IMessagePublisher _messagePublisher;
    private static UserModel? _loggedInUser;
    
    public E2EChatManager(IAuthUtil authUtil, IUserService userService, IAuthService authService, IMessagePublisher messagePublisher)
    {
        _authUtil = authUtil;
        _userService = userService;
        _authService = authService;
        _messagePublisher = messagePublisher;
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
                    Console.WriteLine("Not implemented");
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

    private void Login()
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
            Menu();
        }
        catch (Exception e)
        {
            Task.Delay(2000).Wait();
            Welcome();
        }
    }

    private void Menu()
    { 
        Console.WriteLine("Welcome "+ _loggedInUser.Username);
        while (true)
        {
            Console.WriteLine("Your options: \n1. Messages | 2. Logout");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    // Messages
                    break;
                case "2":
                    Console.WriteLine("Logging out...");
                    Thread.Sleep(1000);
                    _loggedInUser = null;
                    Welcome();
                    break;
                default:
                    Thread.Sleep(1000);
                    continue;
            }
            break;
        }
    }

    private static async Task Messaging()
    {
        Console.WriteLine("Email of the person you want to chat: ");
        var receiver = Console.ReadLine();
        
        var listener = new MessageListener(_loggedInUser);
        var listenerTask = Task.Run(() => listener.Start());
        
        await listenerTask;
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
                Console.WriteLine("Password must be at least 8 characters and must contain small and capital letters, digits and at least one special character.");
                Thread.Sleep(5000);
                Register();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Password must be at least 8 characters and must contain small and capital letters, digits and at least one special character.");
        }
    }
}