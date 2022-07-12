using System.ComponentModel.DataAnnotations;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;

namespace ExampleSendEmailOrWebPushApi.Controllers.Requests;

public class SendWebPushRequest
{
    [Required]
    public string DeviceEndpoint { get; set; }

    [Required]
    public string P256dh { get; set; }

    [Required]
    public string AuthSecret { get; set; }

    [Required]
    public string MessageTitle { get; set; }

    public string MessageText { get; set; }

    public string MessageType { get; set; }

    public PushMessageUrgency? MessageUrgency { get; set; }
}