using System.Security.Cryptography;

namespace e2ee_chat.Util;

public class Crypto
{
    private static Crypto _instance;
    private ECDiffieHellmanCng _cng;
    private byte[] _sharedSecret;
    private byte[] _publicKey;

    private Crypto()
    {
        _cng = new ECDiffieHellmanCng();
        _cng.HashAlgorithm = CngAlgorithm.Sha256;
        _publicKey = _cng.PublicKey.ToByteArray();
    }

    public static Crypto Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Crypto();
            }

            return _instance;
        }
    }

    public byte[] GetPublicKey()
    {
        return _publicKey;
    }

    public void GenerateSharedSecret(byte[] publicKeyPartner)
    {
        _sharedSecret = _cng.DeriveKeyMaterial(CngKey.Import(publicKeyPartner, CngKeyBlobFormat.EccPublicBlob));
    }

    public byte[] GetSharedSecret()
    {
        return _sharedSecret;
    }

    private byte[] DeriveKeyChain(byte[] key, byte[] userSalt)
    {
        using (var kdf = new Rfc2898DeriveBytes(key,userSalt,256/8,HashAlgorithmName.SHA512))
        {
            return kdf.GetBytes(256 / 8);
        }
    }

    public byte[] Encrypt(string plaintxtMsg)
    {
        try
        {
            
            using var aes = Aes.Create();

            aes.Key = _sharedSecret;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plaintxtMsg);
                    }
                }

                return ms.ToArray();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string Decrypt(byte[] ciphertxtMsg)
    {
        try
        {
            using var aes = Aes.Create();

            aes.Key = _sharedSecret;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(ciphertxtMsg, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(ciphertxtMsg, iv.Length, ciphertxtMsg.Length - iv.Length))
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}