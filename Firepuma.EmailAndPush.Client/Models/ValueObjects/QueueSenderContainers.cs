using Azure.Messaging.ServiceBus;

namespace Firepuma.EmailAndPush.Client.Models.ValueObjects;

public class QueueSenderContainers
{
    public ServiceBusSender EmailSender { get; init; }
    public ServiceBusSender WebPushSender { get; init; }
}