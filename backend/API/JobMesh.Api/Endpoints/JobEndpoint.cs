using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using System.Security.Claims;
using JobMesh.Api.Data;
using JobMesh.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace JobMesh.Api.Endpoints;

public static class JobEndpoints
{
    private static ILogger<Program>? _logger;

    public static WebApplication MapJobEndpoints(this WebApplication app)
    {
        _logger = app.Services.GetRequiredService<ILogger<Program>>();
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
        app.MapGet("/api/my/jobs", GetUserJobsAsync).RequireAuthorization();
        return app;
    }

    private static async Task<IResult> SubmitJobAsync(
        HttpContext httpContext,
        JobSubmission submission,
        JobService jobService,
        UserService userService)
    {
        _logger?.LogInformation("[Job] Submitting new job...");

        var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            _logger?.LogWarning("[Job] Unauthorized - no username in claims");
            return Results.Unauthorized();
        }

        var user = await userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            _logger?.LogWarning($"[Job] User not found: {username}");
            return Results.Unauthorized();
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = submission.JobType,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };

        _logger?.LogInformation($"[Job] Created job object: {job.Id}");

        // Save to database
        var saved = await jobService.CreateJobAsync(job);

        if (saved == null)
        {
            _logger?.LogError("[Job] Failed to save job to database");
            return Results.BadRequest(new { Message = "Failed to save job to database" });
        }

        _logger?.LogInformation($"[Job] ✅ Job saved to database: {job.Id}");

        // Enqueue to Redis
        try
        {
            await redisQueueService.EnqueueAsync("job_queue", job.Id.ToString());
            _logger?.LogInformation($"[Job] ✅ Job enqueued to Redis: {job.Id}");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"[Job] ⚠️ Failed to enqueue to Redis: {ex.Message}");
            // Don't fail - job is saved in database
        }

        return Results.Ok(new { Id = job.Id, Message = "Job submitted successfully!" });
    }

    private static async Task<IResult> GetUserJobsAsync(
        HttpContext httpContext,
        JobService jobService,
        UserService userService)
    {
        var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Results.Unauthorized();
        }

        var user = await userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var jobs = await jobService.GetUserJobsAsync(user.Id);
        return Results.Ok(jobs);
    }
}