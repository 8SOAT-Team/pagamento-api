using Pagamentos.Adapters.DependencyInjection;
using Pagamentos.Api;
using Pagamentos.Api.Endpoints;
using Pagamentos.Api.Services;
using Pagamentos.Infrastructure.Configurations;
using Pagamentos.Infrastructure.DependencyInjection;
using Serilog;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5271);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUseCaseControllers()
    .AddGateways()
    .AddGatewayDePagamento()
    .AddDatabaseContext()
    .AddCacheService()
    .ConfigureJsonSerialization();

builder.Services.AddLogging(logBuilder =>
{
    logBuilder.AddSerilog(new LoggerConfiguration()
        .WriteTo
        .Async(config => config.Console())
        .CreateLogger());
});

builder.Services.AddHealthChecks();

var app = builder.Build();

//Executar as migracoes pendentes
if (EnvConfig.RunMigrationsOnStart)
{
    await MigracoesPendentes.ExecuteMigrationAsync(app);
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fast Order v1");
    options.RoutePrefix = string.Empty;
});

app.AddEndpointPagamentos();
app.AddEndpointWebhook();
app.UseHealthChecks("/health");

app.Run();

public partial class Program { }