using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Interfaces.Repositories;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }
    
    public void CreateUser(UserModel user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentException("Contact support.");
            }
            _repo.CreateUser(user);
        }
        catch (Exception e)
        {
            Console.WriteLine("Contact support.");
        }
    }

    public UserModel GetUser(string email)
    {
        try
        {
            return _repo.GetUser(email);
        }
        catch (Exception e)
        {
            Console.WriteLine("Wrong email or password.");
            throw;
        }
    }
}