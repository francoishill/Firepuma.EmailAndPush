using Firepuma.EmailAndPush.Abstractions.Models.Dtos.HttpRequests;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos.ServiceBusMessages;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;
using Firepuma.EmailAndPush.Client.Models.ValueObjects;

namespace Firepuma.EmailAndPush.Client.Services;

public interface IEmailAndPushClient
{
    Task<SuccessOrFailure<SuccessfulResult, FailedResult>> EnqueueEmail(SendEmailRequestDto requestDto, CancellationToken cancellationToken);
    
    Task<SuccessOrFailure<SuccessfulResult, FailedResult>> AddWebPushDevice(AddWebPushDeviceRequestDto requestDto, CancellationToken cancellationToken);
    Task<SuccessOrFailure<SuccessfulResult, FailedResult>> EnqueueWebPush(SendWebPushRequestDto requestDto, CancellationToken cancellationToken);
}