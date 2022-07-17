using System.Net;
using Azure.Messaging.ServiceBus;
using Firepuma.EmailAndPush.Abstractions.Infrastructure.Validation;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos.HttpRequests;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos.ServiceBusMessages;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;
using Firepuma.EmailAndPush.Client.Models.ValueObjects;
using Newtonsoft.Json;

namespace Firepuma.EmailAndPush.Client.Services;

public class ServiceBusEmailAndPushClient : IEmailAndPushClient
{
    public const string HTTP_CLIENT_NAME = "Firepuma.EmailAndPush.Client.Services.ServiceBusEmailAndPushClient";

    private readonly QueueSenderContainers _senders;
    private readonly HttpClient _httpClient;

    public ServiceBusEmailAndPushClient(
        QueueSenderContainers senders,
        IHttpClientFactory httpClientFactory)
    {
        if (senders.EmailSender == null) throw new ArgumentException("EmailSender is not but required");
        if (senders.WebPushSender == null) throw new ArgumentException("WebPushSender is not but required");

        _senders = senders;
        _httpClient = httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
    }

    public async Task<SuccessOrFailure<SuccessfulResult, FailedResult>> EnqueueEmail(
        SendEmailRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResults))
        {
            return new FailedResult(FailedResult.FailedReason.InputValidationFailed, validationResults.Select(r => r.ErrorMessage).ToArray());
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

        return new SuccessfulResult();
    }

    public async Task<SuccessOrFailure<SuccessfulResult, FailedResult>> AddWebPushDevice(AddWebPushDeviceRequestDto requestDto, CancellationToken cancellationToken)
    {
        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResults))
        {
            return new FailedResult(FailedResult.FailedReason.InputValidationFailed, validationResults.Select(r => r.ErrorMessage).ToArray());
        }

        var response = await _httpClient.PutAsync("/api/AddWebPushDevice", new StringContent(JsonConvert.SerializeObject(requestDto)), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new FailedResult(
                    FailedResult.FailedReason.InputValidationFailed,
                    TryDeserializeErrorsResponseDto(responseBody, out var errorsResponseDto) ? errorsResponseDto.Errors : new[] { responseBody });
            }

            return new FailedResult(FailedResult.FailedReason.Unknown, new[] { responseBody });
        }

        return new SuccessfulResult();
    }

    public async Task<SuccessOrFailure<SuccessfulResult, FailedResult>> EnqueueWebPush(
        SendWebPushRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResults))
        {
            return new FailedResult(FailedResult.FailedReason.InputValidationFailed, validationResults.Select(r => r.ErrorMessage).ToArray());
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

        return new SuccessfulResult();
    }

    private static bool TryDeserializeErrorsResponseDto(string responseBody, out ErrorsResponseDto errorsResponseDto)
    {
        try
        {
            errorsResponseDto = JsonConvert.DeserializeObject<ErrorsResponseDto>(responseBody);
            return true;
        }
        catch (Exception)
        {
            errorsResponseDto = null;
            return false;
        }
    }
}