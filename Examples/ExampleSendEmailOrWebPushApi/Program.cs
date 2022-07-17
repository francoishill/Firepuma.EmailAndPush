using System.Reflection;
using Firepuma.EmailAndPush.Client;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Configuration, builder.Services);

var app = builder.Build();
ConfigureApp(app);
app.Run();

static void ConfigureServices(ConfigurationManager configuration, IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Example API",
            Description = "An example web api to demonstrate SendEmailOrWebPush service",
            Contact = new OpenApiContact
            {
                Name = "Firepuma.EmailAndPush",
                Url = new Uri("https://github.com/francoishill/Firepuma.EmailAndPush")
            },
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

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