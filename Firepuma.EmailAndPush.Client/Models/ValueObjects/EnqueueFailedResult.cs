namespace Firepuma.EmailAndPush.Client.Models.ValueObjects;

public class EnqueueFailedResult
{
    public FailedReason Reason { get; set; }
    public string[] Errors { get; set; }

    public EnqueueFailedResult(FailedReason reason, string[] errors)
    {
        Reason = reason;
        Errors = errors;
    }

    public enum FailedReason
    {
        InputValidationFailed,
    }
}