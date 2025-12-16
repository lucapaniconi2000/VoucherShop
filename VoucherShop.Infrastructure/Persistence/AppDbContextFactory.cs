using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VoucherShop.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string ConnectionArg = "--connection";

    public AppDbContext CreateDbContext(string[] args)
    {
        // 1) Se passata da CLI: --connection "Host=...;Database=...;"
        var cliCs = TryGetArgValue(args, ConnectionArg);
        if (!string.IsNullOrWhiteSpace(cliCs))
            return Create(cliCs);

        // 2) Carica configurazione con più fallback robusti
        var config = BuildConfiguration();

        var cs =
            config.GetConnectionString("DefaultConnection")
            ?? config["ConnectionStrings:DefaultConnection"]
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(cs))
        {
            throw new InvalidOperationException(
                "Design-time connection string not found.\n" +
                "Provide it via:\n" +
                "  - appsettings.json (Api)\n" +
                "  - env var ConnectionStrings__DefaultConnection\n" +
                "  - CLI: dotnet ef ... -- --connection \"Host=...;...\""
            );
        }

        return Create(cs);
    }

    private static AppDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Directory da cui lanci il comando
        var basePath = Directory.GetCurrentDirectory();

        // Possibili path dove può stare appsettings.json
        var candidates = new[]
        {
            // se sei già dentro VoucherShop.Api
            basePath,

            // se sei nella root solution
            Path.Combine(basePath, "VoucherShop.Api"),

            // se sei dentro VoucherShop.Infrastructure
            Path.Combine(basePath, "..", "VoucherShop.Api"),
            Path.Combine(basePath, "..", "..", "VoucherShop.Api"),

            // se sei in una cartella ancora più profonda
            Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "VoucherShop.Api")),
        };

        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        // Prova a caricare appsettings.json e appsettings.{env}.json da tutte le candidate
        foreach (var path in candidates.Distinct().Where(Directory.Exists))
        {
            builder.SetBasePath(path);

            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            builder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
        }

        return builder.Build();
    }

    private static string? TryGetArgValue(string[] args, string key)
    {
        // Formati supportati:
        // --connection "..."
        // --connection=...
        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];

            if (string.Equals(a, key, StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length)
                    return args[i + 1];

                return null;
            }

            if (a.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
            {
                return a[(key.Length + 1)..];
            }
        }

        return null;
    }
}
