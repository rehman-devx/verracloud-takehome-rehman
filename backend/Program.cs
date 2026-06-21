using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Repositories;
using Backend.Services;
using Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database — tell EF Core to use SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=holdings.db"));

// CORS — allow React frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// SignalR — for real time updates
builder.Services.AddSignalR();

// Repositories
builder.Services.AddScoped<IHoldingRepository, HoldingRepository>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();

// Services
builder.Services.AddScoped<IHoldingService, HoldingService>();
builder.Services.AddHostedService<PriceUpdateService>();

var app = builder.Build();

// Auto create and migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.MapHub<PriceHub>("/hubs/prices");  // ← this was missing

app.Run();