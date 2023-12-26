using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Interfaces;

public interface IAuthUtil
{
    UserModel PasswordHasher(string email, string username, string password);
    //void GenerateSharedSecret(byte[] publicKey, string receiver);
}