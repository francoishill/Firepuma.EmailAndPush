using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Firepuma.EmailAndPush.Abstractions.Infrastructure.Validation;
using Firepuma.EmailAndPush.Abstractions.Models.Dtos.HttpRequests;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Factories;
using Firepuma.EmailAndPush.FunctionApp.Models.TableModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firepuma.EmailAndPush.FunctionApp.Input;

public static class AddWebPushDevice
{
    [FunctionName("AddWebPushDevice")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
        ILogger log,
        [Table("WebPushDevices")] CloudTable webPushDevicesTable,
        CancellationToken cancellationToken)
    {
        log.LogInformation("C# HTTP trigger function processed a request");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var requestDto = JsonConvert.DeserializeObject<AddWebPushDeviceRequestDto>(requestBody);

        if (!ValidationHelpers.ValidateDataAnnotations(requestDto, out var validationResultsForRequest))
        {
            return HttpResponseFactory.CreateBadRequestResponse(new[] { "Request body is invalid." }.Concat(validationResultsForRequest.Select(s => s.ErrorMessage)).ToArray());
        }

        var webPushDevice = new WebPushDevice(
            requestDto.ApplicationId,
            requestDto.DeviceId,
            requestDto.UserId,
            requestDto.DeviceEndpoint,
            requestDto.P256dh,
            requestDto.AuthSecret);

        try
        {
            await webPushDevicesTable.ExecuteAsync(TableOperation.Insert(webPushDevice), cancellationToken);
        }
        catch (StorageException storageException) when (storageException.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
        {
            return HttpResponseFactory.CreateBadRequestResponse($"The device (id '{requestDto.DeviceId}' and application id '{requestDto.ApplicationId}') is already added and cannot be added again");
        }

        return new OkResult();
    }
}