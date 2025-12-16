using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VoucherShop.Application.Interfaces;
using VoucherShop.Infrastructure.Auth;

namespace VoucherShop.Infrastructure.Extensions;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
