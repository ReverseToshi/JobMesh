using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using System.Security.Claims;
using JobMesh.Api.Data;
using JobMesh.Api.Services;
using Microsoft.EntityFrameworkCore;  // ← Add this

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
        app.MapPost("/api/jobs", async (HttpContext httpContext, JobSubmission submission, JobService jobService) =>
        {
            var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }

            var job = new Job
            {
                Id = Guid.NewGuid(),
                UserId = username,
                Type = submission.JobType,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
            };

            var saved = await jobService.CreateJobAsync(job);  // ← Use await!

            if (saved == null)
            {
                return Results.BadRequest(new { Message = "Failed to save job to database" });
            }

            Console.WriteLine($"Job submitted: {job.Id}, Type: {job.Type}, UserId: {job.UserId}");
            return Results.Ok(new { Id = job.Id, Message = "Job submitted successfully!" });
        }).RequireAuthorization();

        return app;
    }

    public static WebApplication MapGetUserJobsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/my/jobs", async (HttpContext httpContext, JobService jobService) =>
        {
            var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }

            var jobs = await jobService.GetUserJobsAsync(username);  // ← Use await!

            return Results.Ok(jobs);
        }).RequireAuthorization();

        return app;
    }
}