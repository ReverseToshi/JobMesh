using JobMesh.Api.Business;
using JobMesh.Api.Models;
using JobMesh.Api.Infrastructure;
using System.Security.Claims;

namespace JobMesh.Api.Endpoints;

public static class GetUserJobsEndpoint{
    public static WebApplication MapGetUserJobsEndpoint(this WebApplication app){
        app.MapGet("/api/my/jobs", (HttpContext httpContext) =>
        {
            var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }

            var mySQLHandler = new MySQLHandler();
            var jobs = mySQLHandler.GetUserJobs(username);

            return Results.Ok(jobs);
        }
        ).RequireAuthorization();
        return app;
    }
}