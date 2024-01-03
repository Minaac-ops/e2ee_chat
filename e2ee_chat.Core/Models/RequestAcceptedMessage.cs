namespace e2ee_chat.Core.Models;

public class RequestAcceptedMessage
{
    public byte[] PublicKey { get; set; }
    public string Publisher { get; set; }
    public string Receiver { get; set; }
}