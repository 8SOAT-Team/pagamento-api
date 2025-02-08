using Microsoft.EntityFrameworkCore;
using Pagamento.Infrastructure.Databases;
using Serilog;

namespace Pagamento.Services;

public static class MigracoesPendentes
{
    public static async Task ExecuteMigrationAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var serviceDb = serviceScope.ServiceProvider
                         .GetService<PagamentoContext>();
        try
        {
            await serviceDb!.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Information(ex.Message);
            var conn = serviceDb!.Database.GetConnectionString();
            throw new Exception($"connection: {conn}", ex);
        }
    }
}