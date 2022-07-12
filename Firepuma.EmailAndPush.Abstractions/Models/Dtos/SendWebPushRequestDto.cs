using System.ComponentModel.DataAnnotations;
using Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Firepuma.EmailAndPush.Abstractions.Models.Dtos;

public class SendWebPushRequestDto
{
    [Required]
    public string ApplicationId { get; set; }
    
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

    // public string MessageUniqueTopicId { get; set; } //TODO: could not get topic working, tested Chrome and MSEdge browsers
}