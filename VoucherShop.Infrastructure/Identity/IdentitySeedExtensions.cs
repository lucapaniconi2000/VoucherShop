using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace VoucherShop.Infrastructure.Identity;

public static class IdentitySeedExtensions
{
    public static async Task SeedIdentityAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AppUser>>();

        // --------------------
        // RUOLI
        // --------------------
        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));
            }
        }

        // --------------------
        // ADMIN USER
        // --------------------
        const string adminEmail = "admin@vouchershop.local";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (!result.Succeeded)
                throw new Exception(
                    $"Admin creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // assegna ruolo Admin se manca
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
