using JobMesh.Api.Models;

namespace JobMesh.Api.Endpoints;

public static class JobSubmissionEndpoints
{
    public static WebApplication MapJobSubmissionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/submitjob", (JobSubmission submission) =>
        {
            Console.WriteLine($"Received job submission: {submission.Id}, Type: {submission.JobType}, Priority: {submission.Priority}");
            return Results.Ok(new { Message = "Job submitted successfully!", Submission = submission });
        });

        return app;
    }
}

