using Pagamento.Adapters.Controllers;
using Pagamento.Configurations;
using Pagamento.Endpoints;
using Pagamento.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPagamentoController, PagamentoController>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

//Executar as migrações pendentes
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