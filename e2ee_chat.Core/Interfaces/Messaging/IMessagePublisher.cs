namespace e2ee_chat.Core.Interfaces.Messaging;

public interface IMessagePublisher
{
    public void PublishMessage(string username, string plaintextMsg);
}