// ReSharper disable UnusedMember.Global

namespace Firepuma.EmailAndPush.Client.Config;

public class EmailAndPushClientOptions
{
    public string SendEmailConnectionString { get; set; }
    public string SendEmailQueueName { get; set; }

    public string SendWebPushConnectionString { get; set; }
    public string SendWebPushQueueName { get; set; }
}