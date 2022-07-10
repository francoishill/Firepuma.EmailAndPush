using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Firepuma.EmailAndPush.FunctionApp.Config;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Exceptions;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Helpers;
using Firepuma.EmailAndPush.FunctionApp.Models.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebPush;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Local

namespace Firepuma.EmailAndPush.FunctionApp.Commands;

public static class SendWebPush
{
    public class Command : IRequest<Result>
    {
        public string DeviceEndpoint { get; init; }
        public string P256dh { get; init; }
        public string AuthSecret { get; init; }

        public string MessageTitle { get; init; }
        public string MessageText { get; init; }
        public string MessageType { get; init; }

        public PushMessageUrgency? MessageUrgency { get; init; }

        // public string MessageUniqueTopicId { get; init; } //TODO: could not get topic working, tested Chrome and MSEdge browsers
    }

    public class Result
    {
        // Empty result for now
    }


    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IOptions<WebPushOptions> _options;

        public Handler(
            IOptions<WebPushOptions> options)
        {
            _options = options;
        }

        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var deviceEndpoint = command.DeviceEndpoint;
            var p256dh = command.P256dh;
            var authSecret = command.AuthSecret;

            var pushSubscription = new PushSubscription(deviceEndpoint, p256dh, authSecret);

            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails(_options.Value.PushApplicationIdentifier, _options.Value.PushPublicKey, _options.Value.PushPrivateKey);
            webPushClient.SetVapidDetails(vapidDetails);

            try
            {
                var pushClientPayload = new Dictionary<string, string>
                {
                    ["title"] = command.MessageTitle,
                };

                if (!string.IsNullOrWhiteSpace(command.MessageText))
                {
                    pushClientPayload.Add("text", command.MessageText);
                }

                if (!string.IsNullOrWhiteSpace(command.MessageType))
                {
                    pushClientPayload.Add("type", command.MessageType);
                }

                var additionalHeaders = new Dictionary<string, object>();

                if (command.MessageUrgency.HasValue)
                {
                    // https://developers.google.com/web/fundamentals/push-notifications/web-push-protocol#urgency
                    additionalHeaders.Add("Urgency", command.MessageUrgency.Value.GetEnumDescriptionOrNull() ?? command.MessageUrgency.ToString());
                }

                //TODO: could not get topic working, tested Chrome and MSEdge browsers
                // if (!string.IsNullOrWhiteSpace(command.MessageUniqueTopicId))
                // {
                //     // https://web.dev/push-notifications-web-push-protocol/#topic
                //     additionalHeaders.Add("Topic", command.MessageUniqueTopicId);
                // }

                await webPushClient.SendNotificationAsync(
                    pushSubscription,
                    JsonConvert.SerializeObject(pushClientPayload),
                    new Dictionary<string, object>
                    {
                        ["headers"] = additionalHeaders
                    },
                    cancellationToken);

                return new Result();
            }
            catch (WebPushException exception)
            {
                if (exception.StatusCode == HttpStatusCode.Gone)
                {
                    throw new WebPushDeviceGoneException($"Push device endpoint '{deviceEndpoint}.ToString()}}' does not exist anymore");
                }

                throw;
            }
        }
    }
}