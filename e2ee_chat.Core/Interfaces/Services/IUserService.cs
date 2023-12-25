using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Interfaces.Services;

public interface IUserService
{
    void CreateUser(UserModel user);
    UserModel GetUser(string email);
}