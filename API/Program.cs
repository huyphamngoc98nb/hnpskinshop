using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SkiShop.API.Middleware;
using Core.Entities;
using SkiShop.Core.Interfaces;
using SkiShop.Infrastructure.Data;
using StackExchange.Redis;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // Add controllers service
builder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Add DbContext service

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Add scoped service for IProductRepository
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped(typeof(IGenericsRepository<>), typeof(GenericsRepository<>));// Add scoped service for IGenericsRepository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); 
builder.Services.AddCors();

builder.Services.AddSingleton<IConnectionMultiplexer>(config => {
    var conn = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Cannot get Redis connection string");
        
    var configuration = ConfigurationOptions.Parse(conn, true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<ICartService, CartService>();

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>().AddEntityFrameworkStores<StoreContext>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => x.WithOrigins("https://localhost:4200","https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()); // Add CORS policy

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>(); // Add custom exception middleware

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index", "Fallback");


try {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex) {
    Console.WriteLine(ex.Message);
    throw;
}
app.Run();
