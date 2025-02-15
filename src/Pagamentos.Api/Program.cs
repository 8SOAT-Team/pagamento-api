using Pagamentos.Adapters.DependencyInjection;
using Pagamento.Endpoints;
using Pagamento.Services;
using Pagamentos.Api;
using Pagamentos.Infrastructure.Configurations;
using Pagamentos.Infrastructure.DependencyInjection;
using Serilog;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUseCaseControllers()
    .AddGateways()
    .AddGatewayDePagamento()
    .AddDatabaseContext()
    .AddCacheService()
    .ConfigureJsonSerialization();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

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

app.UseHttpsRedirection();

app.Run();

public partial class Program { }