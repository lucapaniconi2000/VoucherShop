using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VoucherShop.Infrastructure.Identity;
using VoucherShop.Infrastructure.Persistence;

namespace VoucherShop.Infrastructure.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}