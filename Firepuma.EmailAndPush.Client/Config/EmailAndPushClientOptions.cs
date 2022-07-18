// ReSharper disable UnusedMember.Global

using System.ComponentModel.DataAnnotations;

namespace Firepuma.EmailAndPush.Client.Config;

public class EmailAndPushClientOptions
{
    [Required]
    public string FunctionAppBaseUrl { get; set; }

    public string FunctionAppSecretCode { get; set; }

    [Required]
    public string SendEmailConnectionString { get; set; }

    [Required]
    public string SendEmailQueueName { get; set; }

    [Required]
    public string SendWebPushConnectionString { get; set; }

    [Required]
    public string SendWebPushQueueName { get; set; }
}