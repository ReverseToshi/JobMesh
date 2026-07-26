using System;
using System.Text;
using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using JobMesh.Api.Data;
using JobMesh.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace JobMesh.Api.Endpoints;

public static class LoginEndpoint
{

    public static WebApplication MapLoginEndpoint(this WebApplication app)
    {
        app.MapPost("/api/login", async (LoginData loginData, UserService userService) =>
        {

            var user = await userService.GetUserByUsernameAsync(loginData.Username);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            if (!Hash.Verify(loginData.Password, user.PasswordHash))
                {   
                return Results.Unauthorized();
            }else{
                var token = JwtHandler.GenerateToken(loginData.Username);
                return Results.Ok(new { Token = token });
            }
        });

        app.MapPost("/api/register", async (LoginData loginData, UserService userService) =>
        {
            var passwordHash = Hash.Create(loginData.Password);

            // InsertUser returns a boolean; check the result and handle failure accordingly.
            var user = new User
            {
                Username = loginData.Username,
                PasswordHash = passwordHash
            };
            if (await userService.GetUserByUsernameAsync(loginData.Username) != null)
            {
                return Results.BadRequest(new { Message = "Username already exists" });
            }

            await userService.CreateUserAsync(user);
            return Results.Ok(new { Message = "User registered successfully!" });
        });

        return app;
    }
}

