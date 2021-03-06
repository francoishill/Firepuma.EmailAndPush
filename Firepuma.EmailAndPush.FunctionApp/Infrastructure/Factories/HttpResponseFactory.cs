using Firepuma.EmailAndPush.Abstractions.Models.Dtos.HttpRequests;
using Microsoft.AspNetCore.Mvc;

namespace Firepuma.EmailAndPush.FunctionApp.Infrastructure.Factories;

public static class HttpResponseFactory
{
    public static IActionResult CreateBadRequestResponse(params string[] errors)
    {
        return new BadRequestObjectResult(new ErrorsResponseDto
        {
            Errors = errors,
        });
    }
}