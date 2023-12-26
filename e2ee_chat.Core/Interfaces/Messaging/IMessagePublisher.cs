namespace e2ee_chat.Core.Interfaces.Messaging;

public interface IMessagePublisher
{
    void PublishMessage(string receiver, string publisher, byte[] plaintextMsg);
}