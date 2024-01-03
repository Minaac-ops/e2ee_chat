using System.Security.Cryptography;
using e2ee_chat.Core.Interfaces.Services;
using e2ee_chat.Core.Models;

namespace e2ee_chat.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private byte[] _key;

    public AuthService(IUserService userService)
    {
        _userService = userService;
    }

    public byte[] GetKey()
    {
        return _key;
    }
    
    private bool AuthenticateLogin(UserModel user, string providedPassword)
    {
        try
        {
            using var aes = Aes.Create();
            aes.IV = user.IV;
            
            using var df2 = new Rfc2898DeriveBytes(providedPassword, user.PasswordSalt, 600000, HashAlgorithmName.SHA512);
            aes.Key = df2.GetBytes(256 / 8);
            using var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(user.EncryptedRandom);
            using var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            var decryptedRandomString = sr.ReadToEnd();
            if (decryptedRandomString != user.Random) return false;
            _key = aes.Key;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Wrong username or password.");
            return false;
        }
    }

    public UserModel Login(string? email, string? pass)
    {
        var user = _userService.GetUser(email!);
        var authenticated = AuthenticateLogin(user, pass!);
        if (!authenticated) { 
            throw new InvalidDataException("Wrong username or password.");
        } return user;
    }
}