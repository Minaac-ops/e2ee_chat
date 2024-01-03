// See https://aka.ms/new-console-template for more information

using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Repositories;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Services;
using e2ee_chat.Infrastructure.Converters;
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
                .BuildServiceProvider().GetService<E2EChatManager>();
        }
    }
}