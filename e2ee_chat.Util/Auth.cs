using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using e2ee_chat.Core.Interfaces;
using e2ee_chat.Core.Models;

namespace e2ee_chat.Util;

public class Auth: IAuthUtil
{
    private byte[] _sharedSecret;
    
    public UserModel PasswordHasher(string email, string username, string password)
    {
        GenerateUserCredentials(password, out var iv, out var passwordSalt, out var random, out var randomString);

        var user = new UserModel
        {
            Email = email,
            Username = username,
            PasswordSalt = passwordSalt,
            IV = iv,
            Random = randomString,
            EncryptedRandom = random,
        };
        return user;
    }

    public byte[] GetSharedSecret()
    {
        return _sharedSecret;
    }

    // public void GenerateSharedSecret(byte[] publicKey, string receiver)
    // {
    //     Console.WriteLine("Authutil");
    //     using var dh = new ECDiffieHellmanCng();
    //     dh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
    //     dh.HashAlgorithm = CngAlgorithm.Sha512;
    //     _sharedSecret = dh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
    //     var bytestring = BitConverter.ToString(_sharedSecret);
    //     Console.WriteLine($"Generated shared secret for {receiver}, the shared secret is {bytestring}");
    // }

    private void GenerateUserCredentials(string password, out byte[] iv, out byte[] passwordSalt, out byte[] random, out string randomString)
    {
        randomString = GenerateRandomString();
        using var aes = Aes.Create();
        aes.GenerateIV();
        
        using var rng = new RNGCryptoServiceProvider();
        // generating a random salt
        var salt = new byte[32];
        rng.GetBytes(salt);

        // use Rfc with 600.000 iterations og sha for password hashing.
        using var df2 = new Rfc2898DeriveBytes(password, salt, 600000, HashAlgorithmName.SHA512);
        aes.Key = df2.GetBytes(256/8);
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(randomString);
                }
                random = ms.ToArray();
            }
        }
        iv = aes.IV;
        passwordSalt = salt;
    }

    private string GenerateRandomString()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var sb = new StringBuilder(12);
        for (var i = 0; i < 12; i++)
        {
            var index = random.Next(chars.Length);
            sb.Append(chars[index]);
        }
        return sb.ToString();
    }
}