using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using System.Security.Claims;
using JobMesh.Api.Services;

namespace JobMesh.Api.Endpoints;

public static class JobEndpoints
{
    public static WebApplication MapJobEndpoints(this WebApplication app)
    {
        app.MapJobSubmissionEndpoints();
        app.MapGetUserJobsEndpoint();
        return app;
    }

    public static WebApplication MapJobSubmissionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/jobs", SubmitJobAsync).RequireAuthorization();
        return app;
    }

    public static WebApplication MapGetUserJobsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/jobs", GetUserJobsAsync).RequireAuthorization();
        return app;
    }

    private static async Task<IResult> SubmitJobAsync(
        HttpContext httpContext,
        JobSubmission submission,
        JobService jobService,
        UserService userService,
        ILogger<Program> logger)
    {
        logger.LogInformation("[Endpoint] POST /api/jobs");

        var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning("[Endpoint] Unauthorized - no username");
            return Results.Unauthorized();
        }

        logger.LogInformation($"[Endpoint] Username from token: {username}");

        // ✅ Look up user
        var user = await userService.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            logger.LogError($"[Endpoint] ❌ User not found in database: {username}");
            return Results.Unauthorized();
        }

        logger.LogInformation($"[Endpoint] ✅ User found: {user.Username} (ID: {user.Id})");

        // ✅ Validate user.Id is not null/empty
        if (string.IsNullOrEmpty(user.Id))
        {
            logger.LogError($"[Endpoint] ❌ User.Id is null or empty for user: {username}");
            return Results.BadRequest(new { Message = "Invalid user ID" });
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,  // ✅ This must exist in Users table
            Type = submission.JobType,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };

        logger.LogInformation($"[Endpoint] Creating job: {job.Id} for user: {user.Id}");

        try
        {
            var saved = await jobService.CreateJobAsync(job);

            if (saved == null)
            {
                logger.LogError("[Endpoint] Job was not saved");
                return Results.BadRequest(new { Message = "Failed to save job" });
            }

            logger.LogInformation($"[Endpoint] ✅ Job created: {job.Id}");
            return Results.Ok(new { Id = job.Id, Message = "Job submitted successfully!" });
        }
        catch (Exception ex)
        {
            logger.LogError($"[Endpoint] ❌ Error creating job: {ex.Message}");
            return Results.BadRequest(new { Message = $"Failed to create job: {ex.Message}" });
        }
    }

    private static async Task<IResult> GetUserJobsAsync(
        HttpContext httpContext,
        JobService jobService,
        UserService userService,
        ILogger<Program> logger)
    {
        logger.LogInformation("[Endpoint] GET /api/my/jobs");

        var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Results.Unauthorized();
        }

        var user = await userService.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            logger.LogWarning($"[Endpoint] User not found: {username}");
            return Results.Unauthorized();
        }

        if (string.IsNullOrEmpty(user.Id))
        {
            logger.LogError($"[Endpoint] User.Id is null for: {username}");
            return Results.BadRequest(new { Message = "Invalid user ID" });
        }

        var jobs = await jobService.GetUserJobsAsync(user.Id);

        return Results.Ok(jobs);
    }
}