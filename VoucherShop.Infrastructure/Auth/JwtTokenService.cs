using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using VoucherShop.Infrastructure.Identity;

namespace VoucherShop.Infrastructure.Auth;

public sealed class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var jwt = _config.GetSection("Jwt");

        var keyBase64 = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var keyBytes = Convert.FromBase64String(keyBase64);
        var key = new SymmetricSecurityKey(keyBytes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),

            // ✅ Tenant / Shop claim
            new("shop_id", user.ShopId.ToString())
        };

        // opzionale ma utile (alcune librerie lo aspettano)
        if (!string.IsNullOrWhiteSpace(user.UserName))
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expiresMinutes = int.TryParse(jwt["ExpiresMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
