using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoucherShop.Application.Interfaces;
using VoucherShop.Infrastructure.Persistence;
using VoucherShop.Infrastructure.Persistence.Audit;
using VoucherShop.Infrastructure.Repositories;

namespace VoucherShop.Infrastructure.Extensions;

public static class DbExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not found");

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        // Repositories (write side)
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IAuditReadRepository, AuditReadRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();


        // Read context (CQRS read side)
        services.AddScoped<IVoucherReadContext>(sp =>
            sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
