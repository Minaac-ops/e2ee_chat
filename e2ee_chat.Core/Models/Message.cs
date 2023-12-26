namespace e2ee_chat.Core.Models;

public class Message
{
    public string Publisher { get; set; }
    public string Receiver { get; set; }
    public byte[] EncryptedMessage { get; set; }
}