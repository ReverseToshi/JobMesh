namespace JobMesh.Api.Models;

public sealed record LoginData
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}