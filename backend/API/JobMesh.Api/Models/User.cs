namespace JobMesh.Api.Models;

public class User
{
    public string Id { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}