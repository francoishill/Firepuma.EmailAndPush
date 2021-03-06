using System;
using System.Threading;
using System.Threading.Tasks;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.EmailAndPush.FunctionApp.Infrastructure.PipelineBehaviors;

public class ExceptionLogBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionLogBehavior<TRequest, TResponse>> _logger;

    public ExceptionLogBehavior(
        ILogger<ExceptionLogBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            var requestTypeName = typeof(TRequest).GetShortTypeName();

            _logger.LogError(
                exception,
                "Failed to perform request {RequestType}, error was: {Error}",
                requestTypeName, exception.Message);

            throw;
        }
    }
}