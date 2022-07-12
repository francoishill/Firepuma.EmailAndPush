using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firepuma.EmailAndPush.Abstractions.Infrastructure.Validation;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos;
using Firepuma.EmailAndPush.FunctionApp.Commands;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Helpers;
using Firepuma.EmailAndPush.FunctionApp.Models.TableModels;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firepuma.EmailAndPush.FunctionApp.Input;

public class SendWebPushTrigger
{
    private readonly IMediator _mediator;

    public SendWebPushTrigger(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("SendWebPushTrigger")]
    public async Task RunAsync(
        [ServiceBusTrigger("%SendWebPushQueueName%", Connection = "SendWebPushServiceBus")] string webPushMessageRequest,
        string messageId,
        ILogger log,
        [Table("UnsubscribedPushDevices")] IAsyncCollector<UnsubscribedPushDevices> unsubscribedDevicesCollector,
        CancellationToken cancellationToken)
    {
        log.LogInformation("C# ServiceBus queue trigger function processing message ID {Id}", messageId);

        var requestDto = JsonConvert.DeserializeObject<SendWebPushRequestDto>(webPushMessageRequest);
        var applicationId = requestDto.ApplicationId;

        log.LogInformation(
            "Processing request for DeviceEndpoint '{DeviceEndpoint}' and ApplicationId '{ApplicationId}'",
            requestDto.DeviceEndpoint, requestDto.ApplicationId);

        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResultsForRequest))
        {
            throw new Exception(string.Join(" ", new[] { "Request body is invalid." }.Concat(validationResultsForRequest.Select(s => s.ErrorMessage))));
        }

        var command = new SendWebPush.Command
        {
            DeviceEndpoint = requestDto.DeviceEndpoint,
            P256dh = requestDto.P256dh,
            AuthSecret = requestDto.AuthSecret,
            MessageTitle = requestDto.MessageTitle,
            MessageText = requestDto.MessageText,
            MessageType = requestDto.MessageType,
            MessageUrgency = requestDto.MessageUrgency,
            // MessageUniqueTopicId = requestDto.MessageUniqueTopicId, //TODO: could not get topic working, tested Chrome and MSEdge browsers
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccessful)
        {
            var failure = result.Failure;

            if (failure.Reason == SendWebPush.FailureReason.DeviceGone)
            {
                log.LogInformation("Push device endpoint does not exist anymore: '{DeviceEndpoint}'", requestDto.DeviceEndpoint);

                var unsubscribeReason = $"{failure.Reason.ToString()} {failure.Message}";
                var unsubscribedDevice = new UnsubscribedPushDevices
                {
                    PartitionKey = applicationId,
                    RowKey = StringUtils.CreateMd5(requestDto.DeviceEndpoint),
                    DeviceEndpoint = requestDto.DeviceEndpoint,
                    UnsubscribeReason = unsubscribeReason,
                };

                await unsubscribedDevicesCollector.AddAsync(unsubscribedDevice, cancellationToken);
            }
            else
            {
                log.LogError(
                    "Failed to send push notification to device endpoint '{DeviceEndpoint}', failure reason '{Reason}', failure message '{Message}'",
                    requestDto.DeviceEndpoint, failure.Reason.ToString(), failure.Message);

                throw new Exception($"Failed to send push notification to device endpoint '{requestDto.DeviceEndpoint}', failure reason '{failure.Reason.ToString()}', failure message '{failure.Message}'");
            }
        }
    }
}