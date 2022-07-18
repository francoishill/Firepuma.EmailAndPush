using System.Text;
using Azure.Messaging.ServiceBus;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos.EventGridMessages;
using Newtonsoft.Json;

// ReSharper disable UnusedParameter.Local
// ReSharper disable SuggestBaseTypeForParameterInConstructor

namespace ExampleSendEmailOrWebPushApi.Services;

public class ServiceBusBackgroundProcessor : AbstractQueueWorker
{
    public ServiceBusBackgroundProcessor(
        ILogger<ServiceBusBackgroundProcessor> logger,
        ServiceBusProcessor serviceBusProcessor)
        : base(logger, serviceBusProcessor)
    {
    }

    protected override async Task ProcessMessage(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
    {
        var messageId = message.MessageId;
        var applicationProperties = message.ApplicationProperties;

        var rawMessageBody = Encoding.UTF8.GetString(message.Body.ToArray());
        Logger.LogInformation(
            "Received message {MessageId} with body {MessageBody} and application properties {ApplicationProperties}",
            messageId, rawMessageBody, applicationProperties);

        var wrappedEventMessage = JsonConvert.DeserializeObject<BaseMessage<object>>(rawMessageBody);
        if (wrappedEventMessage != null
            && !string.IsNullOrWhiteSpace(wrappedEventMessage.Topic)
            && !string.IsNullOrWhiteSpace(wrappedEventMessage.Subject)
            && !string.IsNullOrWhiteSpace(wrappedEventMessage.EventType))
        {
            switch (wrappedEventMessage.EventType)
            {
                case "Firepuma.EmailAndPush.WebPushDeviceUnsubscribed":
                {
                    var eventData = JsonConvert.DeserializeObject<BaseMessage<WebPushDeviceUnsubscribedDto>>(rawMessageBody).Data;
                    await HandleEvent(eventData, cancellationToken);
                    break;
                }

                case "Firepuma.EmailAndPush.NoWebPushDevicesForUser":
                {
                    var eventData = JsonConvert.DeserializeObject<BaseMessage<NoWebPushDevicesForUserDto>>(rawMessageBody).Data;
                    await HandleEvent(eventData, cancellationToken);
                    break;
                }

                default:
                {
                    throw new Exception($"Unknown event type '{wrappedEventMessage.EventType}' for message ID '{messageId}'");
                }
            }
        }
        else
        {
            throw new Exception($"Unknown service bus message (does not have Topic, Subject and EventType) for message ID '{messageId}', raw body: '{rawMessageBody}'");
        }
    }

    private async Task HandleEvent(
        WebPushDeviceUnsubscribedDto eventData,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received WebPushDeviceUnsubscribed event, device ID {DeviceId} and user ID {UserId}", eventData.DeviceId, eventData.UserId);
        await Task.CompletedTask;
    }

    private async Task HandleEvent(
        NoWebPushDevicesForUserDto eventData,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received NoWebPushDevicesForUser event, user ID {UserId}", eventData.UserId);
        await Task.CompletedTask;
    }

    private class BaseMessage<TData>
    {
        public string Topic { get; set; }
        public string Subject { get; set; }
        public string EventType { get; set; }
        public TData Data { get; set; }
    }
}