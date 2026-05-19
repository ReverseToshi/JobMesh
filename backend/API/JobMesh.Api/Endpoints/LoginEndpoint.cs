using System;
using System.Text;
using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;

namespace JobMesh.Api.Endpoints;

public static class LoginEndpoint
{
    public static WebApplication MapLoginEndpoint(this WebApplication app)
    {
        app.MapPost("/api/login", (LoginData loginData) =>
        {
            var mySQLHandler = new MySQLHandler();
            var passwordHash = mySQLHandler.GetUserPasswordHash(loginData.Username);
            
            String hashedPassword = Hash.Create(loginData.Password);
            if (passwordHash == null || !Hash.Verify(loginData.Password, passwordHash)){
                return Results.Unauthorized();
            }else{
                var token = JwtHandler.GenerateToken(loginData.Username);
                return Results.Ok(new { Token = token });
            }
        });

        app.MapPost("/api/register", (LoginData loginData) =>
        {
            var mySQLHandler = new MySQLHandler();
            var passwordHash = Hash.Create(loginData.Password);

            // InsertUser returns a boolean; check the result and handle failure accordingly.
            if (!mySQLHandler.InsertUser(loginData.Username, passwordHash))
            {
                return Results.BadRequest(new { Message = "Failed to register user." });
            }

            return Results.Ok(new { Message = "User registered successfully!" });
        });

        return app;
    }
}

