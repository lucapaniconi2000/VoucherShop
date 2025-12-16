using Microsoft.Extensions.DependencyInjection;
using VoucherShop.Application.Vouchers.Queries.GetMyVoucher;

namespace VoucherShop.Application.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(GetMyVoucherQuery).Assembly
            )
        );

        return services;
    }
}
