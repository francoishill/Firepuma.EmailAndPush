using Firepuma.EmailAndPush.Client.Config;
using Firepuma.EmailAndPush.Client.Models.ValueObjects;
using Firepuma.EmailAndPush.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Firepuma.EmailAndPush.Client;

public static class ServiceCollectionExtensions
{
    public static void AddEmailAndPushClient(
        this IServiceCollection services,
        IConfiguration emailAndPushClientConfigurationSection)
    {
        services.ConfigureAndValidate<EmailAndPushClientOptions>(emailAndPushClientConfigurationSection);

        services.AddHttpClient(
            ServiceBusEmailAndPushClient.HTTP_CLIENT_NAME,
            (s, client) =>
            {
                var options = s.GetRequiredService<IOptions<EmailAndPushClientOptions>>();
                var secretCode = options.Value.FunctionAppSecretCode;

                client.BaseAddress = new Uri(options.Value.FunctionAppBaseUrl);
                if (!string.IsNullOrWhiteSpace(secretCode))
                {
                    client.DefaultRequestHeaders.Add("x-functions-key", secretCode);
                }
            });

        services.AddSingleton(s =>
        {
            var options = s.GetRequiredService<IOptions<EmailAndPushClientOptions>>();

            var emailClient = new Azure.Messaging.ServiceBus.ServiceBusClient(options.Value.SendEmailConnectionString);
            var webPushClient = new Azure.Messaging.ServiceBus.ServiceBusClient(options.Value.SendWebPushConnectionString);

            return new QueueSenderContainers
            {
                EmailSender = emailClient.CreateSender(options.Value.SendEmailQueueName),
                WebPushSender = webPushClient.CreateSender(options.Value.SendWebPushQueueName),
            };
        });

        services.AddSingleton<IEmailAndPushClient, ServiceBusEmailAndPushClient>();
    }
}