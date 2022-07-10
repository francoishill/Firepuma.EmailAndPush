using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firepuma.EmailAndPush.FunctionApp.Commands;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Validation;
using Firepuma.EmailAndPush.FunctionApp.Models.Dtos;
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
        CancellationToken cancellationToken)
    {
        log.LogInformation("C# ServiceBus queue trigger function processing message ID {Id}", messageId);

        var requestDto = JsonConvert.DeserializeObject<SendWebPushRequestDto>(webPushMessageRequest);

        log.LogInformation("Processing request for DeviceEndpoint '{DeviceEndpoint}'", requestDto.DeviceEndpoint);

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

        await _mediator.Send(command, cancellationToken);
    }
}