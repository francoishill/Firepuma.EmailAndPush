using Firepuma.EmailAndPush.Abstractions.Models.Dtos;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;
using Firepuma.EmailAndPush.Client.Models.ValueObjects;

namespace Firepuma.EmailAndPush.Client.Services;

public interface IEmailAndPushClient
{
    Task<SuccessOrFailure<EnqueueSuccessfulResult, EnqueueFailedResult>> EnqueueEmail(SendEmailRequestDto requestDto, CancellationToken cancellationToken);
    Task<SuccessOrFailure<EnqueueSuccessfulResult, EnqueueFailedResult>> EnqueueWebPush(SendWebPushRequestDto requestDto, CancellationToken cancellationToken);
}