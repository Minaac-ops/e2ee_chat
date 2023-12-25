using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Interfaces.Services;

public interface IAuthService
{
    UserModel Login(string? email, string? pass);
}