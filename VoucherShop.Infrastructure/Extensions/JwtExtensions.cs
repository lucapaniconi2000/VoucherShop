using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VoucherShop.Infrastructure.Auth;

namespace VoucherShop.Infrastructure.Extensions;
public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");

        // Decodifica Base64 (256 bit reali)
        var keyBytes = Convert.FromBase64String(jwt["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Validazioni fondamentali
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                // Nessuna tolleranza extra sui tempi
                ClockSkew = TimeSpan.Zero,

                // Allineati a appsettings.json
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
            };
        });

        services.AddAuthorization();

        // Service per generare JWT
        services.AddScoped<JwtTokenService>();

        return services;
    }
}

