using System.ComponentModel.DataAnnotations;

namespace ExampleSendEmailOrWebPushApi.Controllers.Requests;

public class AddWebPushDeviceRequest
{
    [Required]
    public string DeviceId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string DeviceEndpoint { get; set; }

    [Required]
    public string P256dh { get; set; }

    [Required]
    public string AuthSecret { get; set; }
}