namespace VoucherShop.Api.Extensions;

public static class CorsExtensions
{
    private const string FePolicy = "FrontendPolicy";

    public static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(FePolicy, policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200" // Angular dev
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // 🔑 refresh token cookie
            });
        });

        return services;
    }

    public static WebApplication UseFrontendCors(this WebApplication app)
    {
        app.UseCors(FePolicy);
        return app;
    }
}
