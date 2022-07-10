using AutoMapper;
using Firepuma.EmailAndPush.FunctionApp;
using Firepuma.EmailAndPush.FunctionApp.Config;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Helpers;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.PipelineBehaviors;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Firepuma.EmailAndPush.FunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        AddAutoMapper(services);
        AddMediator(services);

        AddWebPush(services);
    }

    private static void AddMediator(IServiceCollection services)
    {
        services.AddMediatR(typeof(Startup));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionLogBehavior<,>));
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Startup));
        services.BuildServiceProvider().GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
    }

    private static void AddWebPush(IServiceCollection services)
    {
        var webPushPublicKey = EnvironmentVariableHelpers.GetRequiredEnvironmentVariable("WebPushPublicKey");
        var webPushPrivateKey = EnvironmentVariableHelpers.GetRequiredEnvironmentVariable("WebPushPrivateKey");

        services.Configure<WebPushOptions>(opt =>
        {
            opt.PushPublicKey = webPushPublicKey;
            opt.PushPrivateKey = webPushPrivateKey;
        });
    }
}