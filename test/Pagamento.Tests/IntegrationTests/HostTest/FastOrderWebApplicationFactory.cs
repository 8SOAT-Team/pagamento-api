using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pagamento.Infrastructure.Databases;
using Testcontainers.MsSql;

namespace Pagamento.Tests.IntegrationTests.HostTest;

public class FastOrderWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IServiceScope? _scope;

    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public PagamentoContext? Context { get; private set; }

    public async Task InitializeAsync()
    {
        await _mssqlContainer.StartAsync();
        _scope = Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<PagamentoContext>();
    }

    public FastOrderWebApplicationFactory()
    {
        DotNetEnv.Env.TraversePath().Load();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<PagamentoContext>));

            services.AddDbContext<PagamentoContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(_mssqlContainer.GetConnectionString()));
        });
    }

    public new async Task DisposeAsync()
    {
        if (_scope != null)
        {
            _scope.Dispose();
        }

        await _mssqlContainer.DisposeAsync();
    }
}