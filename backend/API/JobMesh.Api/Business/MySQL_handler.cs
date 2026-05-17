using MySql.Data.MySqlClient;

namespace JobMesh.Api.Business;

public class MySQLHandler
{
    private readonly string _connectionString;

    public MySQLHandler()
    {
        var host = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
        var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
        var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";
        var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "database";

        _connectionString = $"Server={host};Port={port};User ID={user};Password={password};Database={database};";
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public void TestConnection()
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();
            Console.WriteLine("Successfully connected to MySQL database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to MySQL database: {ex.Message}");
        }
    }

    public void createSchema()
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();

            var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS users (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    username VARCHAR(255) NOT NULL UNIQUE,
                    password_hash VARCHAR(255) NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ";

            using var command = new MySqlCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            Console.WriteLine("Schema created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create schema: {ex.Message}");
        }
    }

    public Boolean InsertUser(string username, string passwordHash)
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();

            var insertQuery = "INSERT INTO users (username, password_hash) VALUES (@username, @passwordHash);";
            using var command = new MySqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@passwordHash", passwordHash);
            command.ExecuteNonQuery();

            Console.WriteLine("User inserted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to insert user: {ex.Message}");
        }
        return false;
    }

    public string GetUserPasswordHash(string username)
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();

            var selectQuery = "SELECT password_hash FROM users WHERE username = @username;";
            using var command = new MySqlCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@username", username);
            var result = command.ExecuteScalar();

            return result?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve user password hash: {ex.Message}");
            return string.Empty;
        }
    }
}