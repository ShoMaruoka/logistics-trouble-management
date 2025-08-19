using Serilog;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using LogisticsTroubleManagement.Infrastructure.Data;
using LogisticsTroubleManagement.Infrastructure.Repositories;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using LogisticsTroubleManagement.API.Middleware;
using LogisticsTroubleManagement.API.Filters;
using Microsoft.AspNetCore.Mvc;
using LogisticsTroubleManagement.Core.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "物流トラブル管理システム API",
        Version = "v1",
        Description = "物流トラブル管理システムのWeb API"
    });
});

// Add Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Database Seeder
builder.Services.AddScoped<DatabaseSeeder>();

// Add Repository Pattern and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IEffectivenessRepository, EffectivenessRepository>();

// Add Domain Services
builder.Services.AddScoped<IncidentDomainService>();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
// Scan validators from Core assembly explicitly
builder.Services.AddValidatorsFromAssembly(typeof(CreateIncidentDtoValidator).Assembly);
// Use custom validation response instead of automatic 400
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "物流トラブル管理システム API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root URL
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

// Initialize database
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    
    Log.Information("Checking database connection...");
    var canConnect = await context.Database.CanConnectAsync();
    if (!canConnect)
    {
        Log.Error("Cannot connect to database. Please check the connection string and ensure the database server is running.");
        throw new InvalidOperationException("Database connection failed.");
    }
    
    Log.Information("Applying database migrations...");
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("pending changes"))
    {
        Log.Warning("Database has pending model changes. This is expected during development.");
        Log.Information("Please run 'dotnet ef migrations add <MigrationName>' to create a new migration.");
    }
    
    Log.Information("Seeding database...");
    await seeder.SeedAsync();
    
    Log.Information("Database initialization completed successfully.");
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while initializing the database.");
    // In development, we might want to continue even if seeding fails
    if (app.Environment.IsDevelopment())
    {
        Log.Warning("Continuing startup despite database initialization error in development mode.");
    }
    else
    {
        throw;
    }
}

app.Run();
