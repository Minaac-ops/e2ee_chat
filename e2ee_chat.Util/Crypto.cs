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
        _cng.HashAlgorithm = CngAlgorithm.Sha512;
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
        Console.WriteLine($"Generated shared secret for, the shared secret is {BitConverter.ToString(_sharedSecret)}");
    }

    public byte[] GetSharedSecret()
    {
        return _sharedSecret;
    }
}