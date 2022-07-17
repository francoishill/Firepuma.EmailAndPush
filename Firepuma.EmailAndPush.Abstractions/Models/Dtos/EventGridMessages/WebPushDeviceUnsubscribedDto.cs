namespace Firepuma.EmailAndPush.Abstractions.Models.Dtos.EventGridMessages;

public class WebPushDeviceUnsubscribedDto
{
    public string ApplicationId { get; set; }
    public string DeviceId { get; set; }
    public string UserId { get; set; }
}