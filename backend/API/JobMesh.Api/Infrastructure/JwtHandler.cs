namespace JobMesh.Api.Infrastructure;

using System;
using System.Text;

public static class JwtHandler
{
    public static string GenerateToken(string username)
    {
        // Simple base64 token: username|utcTicks. Replace with real JWT logic as needed.
        var payload = $"{username}|{DateTime.UtcNow.Ticks}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    public static string? ValidateToken(string token)
    {
        try
        {
            var payloadBytes = Convert.FromBase64String(token);
            var payload = Encoding.UTF8.GetString(payloadBytes);
            var parts = payload.Split('|');
            if (parts.Length != 2) return null;

            var username = parts[0];
            // Optionally, validate timestamp in parts[1] for token expiration.

            return username;
        }
        catch
        {
            return null; // Invalid token format
        }
    }
}