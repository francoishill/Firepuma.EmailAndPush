using ExampleSendEmailOrWebPushApi.Controllers.Requests;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos;
using Firepuma.EmailAndPush.ServiceBusClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleSendEmailOrWebPushApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly IEmailAndPushClient _emailAndPushClient;

    public NotificationsController(
        ILogger<NotificationsController> logger,
        IEmailAndPushClient emailAndPushClient)
    {
        _logger = logger;
        _emailAndPushClient = emailAndPushClient;
    }

    [HttpPost("email")]
    public async Task<IActionResult> SendEmail(
        [FromBody] SendEmailRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Using library to send an email (in background) via the Firepuma.EmailAndPush.Client library");

        var emailRequest = new SendEmailRequestDto
        {
            ApplicationId = Constants.APPLICATION_ID,
            TemplateId = request.TemplateId,
            TemplateData = request.TemplateData,
            Subject = request.Subject,
            ToEmail = request.ToEmail,
            ToName = request.ToName,
            HtmlBody = request.HtmlBody,
            TextBody = request.TextBody,
        };
        var result = await _emailAndPushClient.EnqueueEmail(emailRequest, cancellationToken);

        if (!result.IsSuccessful)
        {
            return new BadRequestObjectResult(result.Failure.Errors);
        }

        return Accepted();
    }

    [HttpPost("web-push")]
    public async Task<IActionResult> SendWebPush(
        [FromBody] SendWebPushRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Using library to send a web push (in background) via the Firepuma.EmailAndPush.Client library");

        var webPushRequest = new SendWebPushRequestDto
        {
            ApplicationId = Constants.APPLICATION_ID,
            DeviceEndpoint = request.DeviceEndpoint,
            P256dh = request.P256dh,
            AuthSecret = request.AuthSecret,
            MessageTitle = request.MessageTitle,
            MessageText = request.MessageText,
            MessageType = request.MessageType,
            MessageUrgency = request.MessageUrgency,
        };
        var result = await _emailAndPushClient.EnqueueWebPush(webPushRequest, cancellationToken);

        if (!result.IsSuccessful)
        {
            return new BadRequestObjectResult(result.Failure.Errors);
        }

        return Accepted();
    }
}