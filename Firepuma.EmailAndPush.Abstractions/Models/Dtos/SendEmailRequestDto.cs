using System.ComponentModel.DataAnnotations;

namespace Firepuma.EmailAndPush.Abstractions.Models.Dtos;

public class SendEmailRequestDto
{
    [Required]
    public string ApplicationId { get; set; }

    public string TemplateId { get; set; }
    public object TemplateData { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string ToEmail { get; set; }

    public string ToName { get; set; }

    public string HtmlBody { get; set; }

    [Required]
    public string TextBody { get; set; }
}