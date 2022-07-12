using Azure.Messaging.ServiceBus;
using Firepuma.EmailAndPush.Abstractions.Infrastructure.Validation;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;
using Firepuma.EmailAndPush.Client.Models.ValueObjects;
using Newtonsoft.Json;

namespace Firepuma.EmailAndPush.Client.Services;

public class ServiceBusEmailAndPushClient : IEmailAndPushClient
{
    private readonly QueueSenderContainers _senders;

    public ServiceBusEmailAndPushClient(QueueSenderContainers senders)
    {
        if (senders.EmailSender == null) throw new ArgumentException("EmailSender is not but required");
        if (senders.WebPushSender == null) throw new ArgumentException("WebPushSender is not but required");

        _senders = senders;
    }

    public async Task<SuccessOrFailure<EnqueueSuccessfulResult, EnqueueFailedResult>> EnqueueEmail(
        SendEmailRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResults))
        {
            return new EnqueueFailedResult(EnqueueFailedResult.FailedReason.InputValidationFailed, validationResults.Select(r => r.ErrorMessage).ToArray());
        }

        var messageJson = JsonConvert.SerializeObject(requestDto);
        var message = new ServiceBusMessage(messageJson)
        {
            ApplicationProperties =
            {
                ["applicationId"] = requestDto.ApplicationId,
            },
        };

        await _senders.EmailSender.SendMessageAsync(message, cancellationToken);

        return new EnqueueSuccessfulResult();
    }

    public async Task<SuccessOrFailure<EnqueueSuccessfulResult, EnqueueFailedResult>> EnqueueWebPush(
        SendWebPushRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResults))
        {
            return new EnqueueFailedResult(EnqueueFailedResult.FailedReason.InputValidationFailed, validationResults.Select(r => r.ErrorMessage).ToArray());
        }

        var messageJson = JsonConvert.SerializeObject(requestDto);
        var message = new ServiceBusMessage(messageJson)
        {
            ApplicationProperties =
            {
                ["applicationId"] = requestDto.ApplicationId,
            },
        };

        await _senders.WebPushSender.SendMessageAsync(message, cancellationToken);

        return new EnqueueSuccessfulResult();
    }
}