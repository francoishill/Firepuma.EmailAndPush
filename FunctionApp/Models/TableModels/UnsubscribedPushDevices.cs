using Microsoft.Azure.Cosmos.Table;

namespace Firepuma.EmailAndPush.FunctionApp.Models.TableModels;

public class UnsubscribedPushDevices : TableEntity
{
    public string UnsubscribeReason { get; init; }
    public string DeviceEndpoint { get; init; }
}