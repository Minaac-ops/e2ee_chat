using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Interfaces.Repositories;

public interface IUserRepository
{
    void CreateUser(UserModel user);
    UserModel GetUser(string email);
}