using Firepuma.EmailAndPush.ServiceBusClient;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Configuration, builder.Services);

var app = builder.Build();
ConfigureApp(app);
app.Run();

static void ConfigureServices(ConfigurationManager configuration, IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddEmailAndPushClient(configuration.GetSection("EmailAndPush"));
}

static void ConfigureApp(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}