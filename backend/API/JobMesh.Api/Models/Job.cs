namespace JobMesh.Api.Models;

public class Job
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public required string Type { get; set; }   // e.g. "image.resize", "csv.analyse"

    public required string Status { get; set; } // Pending, Running, Completed, Failed

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}