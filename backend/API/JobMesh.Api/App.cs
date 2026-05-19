using JobMesh.Api.Endpoints;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnv(Path.Combine(builder.Environment.ContentRootPath, ".env"));
LoadDotEnv(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var mySQLHandler = new MySQLHandler();
mySQLHandler.TestConnection();
mySQLHandler.createSchema();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddAuthentication("JWT")
    .AddScheme<JwtAuthenticationSchemeOptions, JwtAuthenticationHandler>("JWT", null);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapLoginEndpoint();
app.MapGetUserJobsEndpoint();
app.MapJobSubmissionEndpoints();

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