namespace Firepuma.EmailAndPush.FunctionApp.Config;

public class WebPushOptions
{
    public string PushApplicationIdentifier { get; set; } = "https://firepuma-email-and-push.function-app.firepuma.com";

    public string PushPublicKey { get; set; }

    public string PushPrivateKey { get; set; }
}