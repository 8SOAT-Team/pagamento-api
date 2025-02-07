using Pagamento.Api.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fast Order v1");
    options.RoutePrefix = string.Empty;
});

app.AddEndpointPagamentos();

app.UseHttpsRedirection();

app.Run();