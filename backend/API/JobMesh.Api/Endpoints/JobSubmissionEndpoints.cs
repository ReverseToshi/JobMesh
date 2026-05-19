using JobMesh.Api.Models;
using JobMesh.Api.Business;
using JobMesh.Api.Infrastructure;
using System.Security.Claims;

namespace JobMesh.Api.Endpoints;

public static class JobSubmissionEndpoints
{
    public static WebApplication MapJobSubmissionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/jobs", (HttpContext httpContext, JobSubmission submission) =>
        {
            var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Results.Unauthorized();
            }

            // Create a Job entity from the submission
            var job = new Job
            {
                Id = Guid.NewGuid(),
                UserId = username,
                Type = submission.JobType,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
            };

            // Save to database
            var mySQLHandler = new MySQLHandler();
            var saved = mySQLHandler.InsertJob(job);

            if (!saved)
            {
                return Results.BadRequest(new { Message = "Failed to save job to database" });
            }

            Console.WriteLine($"Job submitted: {job.Id}, Type: {job.Type}, UserId: {job.UserId}");
            return Results.Ok(new { Id = job.Id, Message = "Job submitted successfully!" });
        }).RequireAuthorization();

        return app;
    }
}

