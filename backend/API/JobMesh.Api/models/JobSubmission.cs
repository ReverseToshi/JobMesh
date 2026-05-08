namespace JobMesh.Api.Models;

public sealed record JobSubmission
{
	public string Id { get; init; }
    public string JobType { get; init; } = string.Empty;

    public string Payload { get; init; } = string.Empty;

    public string Priority { get; init; } = "Normal";

    public int RetryCount { get; init; } = 0;

    public DateTime SubmittedAt { get; init; } = DateTime.UtcNow;
}
