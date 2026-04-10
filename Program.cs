using GestorTarefa.Infrastructure.Data;
using GestorTarefa.Infrastructure.Repositories;
using GestorTarefa.Application.Interfaces;
using GestorTarefa.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured JSON logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// DI registrations (Scoped)
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<GestorTarefa.Application.Interfaces.ITaskService, GestorTarefa.Application.Services.TaskService>();
builder.Services.AddScoped<DbSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Try to apply migrations and seed database (with retries for Docker/Postgres readiness)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

    var maxAttempts = 20;
    var delay = TimeSpan.FromSeconds(30);
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Attempt {Attempt} - applying migrations...", attempt);
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");

            logger.LogInformation("Starting database seeding...");
            await seeder.SeedAsync();
            logger.LogInformation("Database seeding finished.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database not ready yet (attempt {Attempt}/{Max}). Retrying in {Delay} seconds...", attempt, maxAttempts, delay.TotalSeconds);
            if (attempt == maxAttempts) throw;
            await Task.Delay(delay);
        }
    }
}

app.Run();
