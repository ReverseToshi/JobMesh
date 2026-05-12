using JobMesh.Api.Models;

namespace JobMesh.Api.Endpoints;

public static class LoginEndpoint
{
    public static WebApplication MapLoginEndpoint(this WebApplication app)
    {
        app.MapPost("/api/login", (LoginData loginData) =>
        {
            Console.WriteLine($"Received login attempt: Username: {loginData.Username}");
            // Here you would typically validate the credentials against a database
            if (loginData.Username == "admin" && loginData.Password == "password")
            {
                return Results.Ok(new { Message = "Login successful!" });
            }
            else
            {
                return Results.Unauthorized();
            }
        });

        return app;
    }
}