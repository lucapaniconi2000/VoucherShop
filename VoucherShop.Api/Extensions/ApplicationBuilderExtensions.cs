using VoucherShop.Api.Middleware;
using VoucherShop.Infrastructure.Identity;

namespace VoucherShop.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task ConfigurePipelineAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // SEED IDENTITY (RUOLI + ADMIN)
        await app.Services.SeedIdentityAsync();
    }
}