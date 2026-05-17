using System;
using System.Security.Cryptography;
using System.Text;

namespace JobMesh.Api.Business;

public static class Hash
{
	private const int Iterations = 100_000;
	private const int HashSize = 32; // 256-bit
	private const int PerUserSaltSize = 16;

	// Creates a password hash using PBKDF2 (HMAC-SHA256).
	// The stored format is: {iterations}.{perUserSaltBase64}.{hashBase64}
	// A global salt is read from the environment variable `SALT` and combined
	// with a per-user random salt for derivation.
	public static string Create(string password)
	{
		if (password is null) throw new ArgumentNullException(nameof(password));

		var envSalt = GetGlobalSalt();

		var globalSalt = Encoding.UTF8.GetBytes(envSalt);

		var perUserSalt = new byte[PerUserSaltSize];
		RandomNumberGenerator.Fill(perUserSalt);

		var combinedSalt = new byte[perUserSalt.Length + globalSalt.Length];
		Buffer.BlockCopy(perUserSalt, 0, combinedSalt, 0, perUserSalt.Length);
		Buffer.BlockCopy(globalSalt, 0, combinedSalt, perUserSalt.Length, globalSalt.Length);

		using var pbkdf2 = new Rfc2898DeriveBytes(password, combinedSalt, Iterations, HashAlgorithmName.SHA256);
		var hash = pbkdf2.GetBytes(HashSize);

		var perUserSaltB64 = Convert.ToBase64String(perUserSalt);
		var hashB64 = Convert.ToBase64String(hash);

		return $"{Iterations}.{perUserSaltB64}.{hashB64}";
	}

	// Verifies a password against the stored hash created by Create().
	public static bool Verify(string password, string stored)
	{
		if (password is null) throw new ArgumentNullException(nameof(password));
		if (string.IsNullOrEmpty(stored)) return false;

		var parts = stored.Split('.', 3);
		if (parts.Length != 3) return false;

		if (!int.TryParse(parts[0], out var iterations)) return false;

		byte[] perUserSalt;
		byte[] expectedHash;
		try
		{
			perUserSalt = Convert.FromBase64String(parts[1]);
			expectedHash = Convert.FromBase64String(parts[2]);
		}
		catch
		{
			return false;
		}

		var envSalt = GetGlobalSalt();

		var globalSalt = Encoding.UTF8.GetBytes(envSalt);

		var combinedSalt = new byte[perUserSalt.Length + globalSalt.Length];
		Buffer.BlockCopy(perUserSalt, 0, combinedSalt, 0, perUserSalt.Length);
		Buffer.BlockCopy(globalSalt, 0, combinedSalt, perUserSalt.Length, globalSalt.Length);

		using var pbkdf2 = new Rfc2898DeriveBytes(password, combinedSalt, iterations, HashAlgorithmName.SHA256);
		var computed = pbkdf2.GetBytes(expectedHash.Length);

		return CryptographicOperations.FixedTimeEquals(computed, expectedHash);
	}

	private static string GetGlobalSalt()
	{
		var envSalt = Environment.GetEnvironmentVariable("SALT");
		if (string.IsNullOrWhiteSpace(envSalt))
		{
			throw new InvalidOperationException(
				"SALT environment variable is not set. Add SALT to backend/API/.env or backend/API/JobMesh.Api/.env, or set it in the process environment.");
		}

		return envSalt;
	}
}

