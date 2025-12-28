using VoucherShop.Api.Extensions;
using VoucherShop.Application.Extensions;
using VoucherShop.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// CORS
builder.Services.AddFrontendCors(builder.Configuration);

// Infrastructure
builder.Services
    .AddDatabase(builder.Configuration)
    .AddIdentityServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddMediatRServices()
    .AddCurrentUser();

var app = builder.Build();

// Pipeline
app.UseFrontendCors();

await app.ConfigurePipelineAsync();

app.Run();
