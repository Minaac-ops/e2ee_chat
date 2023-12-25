namespace e2ee_chat.Core.Interfaces.Messaging;

public interface IMessageListener
{
    Task Listen();
}