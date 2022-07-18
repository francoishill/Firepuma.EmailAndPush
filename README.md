# Firepuma.EmailAndPush

A repository containing code for a microservice to send email and push notifications.


## Overview of projects

* `Firepuma.EmailAndPush.Abstractions` project contains shared models and basic logic, which is references/used by both the Client and FunctionApp.
* `Firepuma.EmailAndPush.Client` project contains the code to send ServiceBus messages to the API/FunctionApp, caters for validating the input DTO and adds `applicationId` to the `ApplicationProperties` of the service bus message.


## Examples

Have a look at the [Examples/README.md](Examples/README.md) for more details on the example projects.


## Credits

* https://stackoverflow.com/a/34636692/1224216