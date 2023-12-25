// See https://aka.ms/new-console-template for more information

using System.Security.Authentication.ExtendedProtection;
using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Messaging;
using e2ee_chat.Core.Interfaces.Repositories;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Services;
using e2ee_chat.Infrastructure;
using e2ee_chat.Infrastructure.Converters;
using e2ee_chat.Infrastructure.Messaging;
using e2ee_chat.Infrastructure.Repositories;
using e2ee_chat.Util;
using Microsoft.Extensions.DependencyInjection;

namespace e2ee_chat
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            
            services
                .AddSingleton<IAuthUtil, Auth>()
                .AddSingleton<UserConverter>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<E2EChatManager>()
                .AddSingleton<IMessagePublisher, MessagePublisher>()
                .BuildServiceProvider().GetService<E2EChatManager>();
        }
    }
}