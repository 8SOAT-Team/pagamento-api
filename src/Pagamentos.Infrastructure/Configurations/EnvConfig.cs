namespace Pagamentos.Infrastructure.Configurations;

public static class EnvConfig
{
    public static string EnvironmentName => EnvConfigValueGetter.MustGetString("ASPNETCORE_ENVIRONMENT");
    public static bool IsTestEnv => EnvironmentName.Equals("test", StringComparison.InvariantCultureIgnoreCase);
    public static string DatabaseConnection => EnvConfigValueGetter.MustGetString("DB_CONNECTION_STRING");
    public static Uri PagamentoWebhookUrl => EnvConfigValueGetter.MustGetUri("PAGAMENTO_WEBHOOK_URL");
    public static string PagamentoFornecedorAccessToken => EnvConfigValueGetter.GetString("PAGAMENTO_FORNECEDOR_ACCESS_TOKEN");
    public static string DistributedCacheUrl => EnvConfigValueGetter.MustGetString("DISTRIBUTED_CACHE_URL");
    public static bool RunMigrationsOnStart => EnvConfigValueGetter.GetBool("RUN_MIGRATIONS_ON_START");

    private static class EnvConfigValueGetter
    {
        public static string MustGetString(string key) => Environment.GetEnvironmentVariable(key) ?? throw new ArgumentNullException(nameof(key));
        public static string GetString(string key) => Environment.GetEnvironmentVariable(key) ?? string.Empty;
        public static Uri MustGetUri(string key)
        {
            var uri = MustGetString(key);
            return new Uri(uri, UriKind.Absolute);
        }

        public static bool GetBool(string key) => bool.TryParse(GetString(key), out var value) && value;
    }
}