using JobMesh.Api.Endpoints;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using JobMesh.Api.Data;
using JobMesh.Api.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnv(Path.Combine(builder.Environment.ContentRootPath, ".env"));
LoadDotEnv(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// Add services to the container
builder.Services.AddOpenApi();
builder.Services
    .AddAuthentication("JWT")
    .AddScheme<JwtAuthenticationSchemeOptions, JwtAuthenticationHandler>("JWT", null);
builder.Services.AddAuthorization();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
});

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JobService>();

// ✅ Redis Configuration
// Priority: appsettings.json > CLOUD_REDIS_URL env var (fallback only)
var redisConnectionString = builder.Configuration.GetSection("Redis")["ConnectionString"];

if (string.IsNullOrEmpty(redisConnectionString))
{
    redisConnectionString = Environment.GetEnvironmentVariable("CLOUD_REDIS_URL");
    Console.WriteLine($"[Redis] Using connection string from CLOUD_REDIS_URL env var");
}
else
{
    Console.WriteLine($"[Redis] Using connection string from appsettings.json");
}

if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        var options = ConfigurationOptions.Parse(redisConnectionString);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 10000;
        options.SyncTimeout = 10000;

        options.Ssl = false; // Enable SSL
        
        var connection = ConnectionMultiplexer.Connect(options);

        connection.ConnectionFailed += (_, args) =>
        {
            Console.WriteLine($"❌ Redis Connection Failed:");
            Console.WriteLine($"Endpoint: {args.EndPoint}");
            Console.WriteLine($"FailureType: {args.FailureType}");
            Console.WriteLine($"Exception: {args.Exception}");
        };

        connection.ConnectionRestored += (_, args) =>
        {
            Console.WriteLine($"✅ Redis Connection Restored: {args.EndPoint}");
        };
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
        
        // Test connection
        if (connection.IsConnected)
        {
            Console.WriteLine("✅ [Redis] Connected successfully");
        }
        else
        {
            Console.WriteLine("⚠️ [Redis] Connection pending...");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ [Redis] Connection failed: {ex.Message}");
        // Add a null connection multiplexer so app doesn't crash
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp => null!);
    }
}
else
{
    Console.WriteLine("❌ [Redis] No connection string configured");
}

builder.Services.AddScoped<RedisQueueService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapLoginEndpoint();
app.MapGetUserJobsEndpoint();
app.MapJobSubmissionEndpoints();
app.MapRedisHealthCheck();  // ← Add health check endpoint

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Run();

static void LoadDotEnv(string envFilePath)
{
    if (!File.Exists(envFilePath))
    {
        return;
    }

    foreach (var rawLine in File.ReadAllLines(envFilePath))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim().Trim('"');

        if (string.IsNullOrEmpty(key))
        {
            continue;
        }

        var existingValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(existingValue))
        {
            continue;
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}

// ✅ Health check endpoint
static class RedisHealthCheck
{
    public static WebApplication MapRedisHealthCheck(this WebApplication app)
    {
        app.MapGet("/api/health/redis", async (RedisQueueService redisService) =>
        {
            try
            {
                var queueLength = await redisService.GetQueueLengthAsync("job_queue");
                return Results.Ok(new
                {
                    status = "healthy",
                    message = "Redis is connected",
                    queueName = "job_queue",
                    queueLength = queueLength,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Redis health check failed: {ex.Message}",
                    statusCode: StatusCodes.Status503ServiceUnavailable
                );
            }
        });

        return app;
    }
}