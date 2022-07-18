# Examples

## ExampleSendEmailOrWebPushApi project

This project contains a rudimentary example to use the EmailAndPush client. The key parts to point out are:

* `Program.cs` file - contains `services.AddEmailAndPushClient` and `AddServiceBusBackgroundProcessor`
* `NotificationsController.cs` file - contains API endpoints to send Email and WebPush via the `_emailAndPushClient`