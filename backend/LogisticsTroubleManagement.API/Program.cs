using Serilog;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using LogisticsTroubleManagement.Infrastructure.Data;
using LogisticsTroubleManagement.Infrastructure.Repositories;
using LogisticsTroubleManagement.Infrastructure.Services;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using LogisticsTroubleManagement.API.Middleware;
using LogisticsTroubleManagement.API.Filters;
using Microsoft.AspNetCore.Mvc;
using LogisticsTroubleManagement.Core.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.CookiePolicy;
using LogisticsTroubleManagement.Core.Services;

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
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
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
    
    // JWT認証の設定をSwaggerに追加
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

// 認証関連のリポジトリ
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Add Master Data Repositories
builder.Services.AddScoped<ITroubleTypeRepository, TroubleTypeRepository>();
builder.Services.AddScoped<IDamageTypeRepository, DamageTypeRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IShippingCompanyRepository, ShippingCompanyRepository>();

// Add Domain Services
builder.Services.AddScoped<IncidentDomainService>();

// Add Authentication Services
builder.Services.AddScoped<IAuthenticationService, LogisticsTroubleManagement.Infrastructure.Services.SimpleAuthenticationService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IJwtService, LogisticsTroubleManagement.Infrastructure.Services.JwtService>();
builder.Services.AddScoped<IPasswordService, LogisticsTroubleManagement.Infrastructure.Services.PasswordService>();

// Add User Management Services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// Add Role Management Services
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();

// Add Password Management Services
builder.Services.AddScoped<IPasswordManagementService, PasswordManagementService>();

// Add Master Data Resolver Service
builder.Services.AddScoped<IMasterDataResolverService, MasterDataResolverService>();

// Add Workflow Validation Service
builder.Services.AddScoped<IWorkflowValidationService, LogisticsTroubleManagement.Infrastructure.Services.WorkflowValidationService>();

// 倉庫担当専用サービス
builder.Services.AddScoped<IWarehouseStaffService, LogisticsTroubleManagement.Infrastructure.Services.WarehouseStaffService>();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
// Scan validators from Core assembly explicitly
builder.Services.AddValidatorsFromAssembly(typeof(CreateIncidentDtoValidator).Assembly);
// Use custom validation response instead of automatic 400
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // クッキーを許可
    });
});

// 認証設定の読み込み
builder.Services.Configure<LogisticsTroubleManagement.Core.Configuration.AuthenticationSettings>(
    builder.Configuration.GetSection("Authentication"));

// 認証・認可サービスの設定
var requireAuth = builder.Configuration.GetValue<bool>("Authentication:RequireAuth", false);

if (requireAuth)
{
    // JWT認証の設定
    var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
    var key = jwtSettings["Key"];
    
    if (string.IsNullOrEmpty(key))
    {
        throw new InvalidOperationException("JWT signing key must be configured via environment variable or Secret Manager");
    }
    
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 基本検証設定
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            // 発行者・対象者・署名キー設定
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            
            // 重要な追加設定（必須）
            ClockSkew = TimeSpan.Zero, // 厳密な時刻検証
            RoleClaimType = ClaimTypes.Role, // ロールクレーム名
            NameClaimType = ClaimTypes.Name, // ユーザー名クレーム名
            ValidateTokenReplay = true, // トークン再利用攻撃防止
            
            // オプション設定
            RequireExpirationTime = true, // 有効期限必須
            RequireSignedTokens = true // 署名必須
        };
        
        // クッキーからのJWTトークン抽出（クッキーベース認証用）
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // クッキーからJWTトークンを抽出
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }
                
                // ヘッダーからの抽出も併用（フォールバック）
                if (string.IsNullOrEmpty(context.Token))
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        context.Token = authHeader.Substring("Bearer ".Length);
                    }
                }
                
                return Task.CompletedTask;
            }
        };
        
        // セキュリティ強化設定
        options.RequireHttpsMetadata = true;
        options.SaveToken = false;
    });
    
    // 認可ポリシーの設定
    builder.Services.AddAuthorization(options => {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ManagerOnly", policy => policy.RequireRole("IncidentManager"));
        options.AddPolicy("WarehouseStaff", policy => policy.RequireRole("WarehouseStaff"));
        options.AddPolicy("ClerkOrAbove", policy => policy.RequireRole("Clerk", "IncidentManager", "WarehouseStaff", "Admin"));
    });
    
    // セッション・クッキー設定
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.Secure = CookieSecurePolicy.Always;
    });
    
    // CSRF保護の設定
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
}

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

// app.UseHttpsRedirection(); // 一時的に無効化

app.UseCors("AllowAll");

// 条件付き認証・認可の適用
if (requireAuth)
{
    app.UseCookiePolicy();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();
    
    // 認証が必要なコントローラーのみに認証を適用
    app.MapControllers();
}
else
{
    // 認証が無効な場合は、認証なしでコントローラーをマップ
    app.MapControllers();
}

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

// Make Program class public for testing
public partial class Program { }
