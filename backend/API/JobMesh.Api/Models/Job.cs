namespace JobMesh.Api.Models;

public class Job
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public required string Type { get; set; }   // e.g. "image.resize", "csv.analyse"

    public required string Status { get; set; } // Pending, Running, Completed, Failed

    public string Payload { get; set; } = string.Empty; // JSON string containing job-specific data

    public string Priority { get; set; } = "Normal"; // Normal, High, Low

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}