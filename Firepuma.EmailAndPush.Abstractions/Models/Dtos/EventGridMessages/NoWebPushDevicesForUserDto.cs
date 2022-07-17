namespace Firepuma.EmailAndPush.Abstractions.Models.Dtos.EventGridMessages;

public class NoWebPushDevicesForUserDto
{
    public string ApplicationId { get; set; }
    public string UserId { get; set; }
}