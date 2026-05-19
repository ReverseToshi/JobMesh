using MySql.Data.MySqlClient;
using JobMesh.Api.Models;

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

                CREATE TABLE Jobs (
                    Id CHAR(36) PRIMARY KEY,
                    UserId VARCHAR(100) NOT NULL,

                    Type VARCHAR(100) NOT NULL,

                    Status VARCHAR(30) NOT NULL,

                    Priority INT DEFAULT 0,

                    Payload JSON NOT NULL,
                    Result JSON NULL,

                    ErrorMessage TEXT NULL,

                    Progress INT DEFAULT 0,

                    RetryCount INT DEFAULT 0,
                    MaxRetries INT DEFAULT 3,

                    AssignedWorkerId VARCHAR(100) NULL,
                    QueueName VARCHAR(100) NULL,

                    ScheduledAt DATETIME NULL,

                    CreatedAt DATETIME NOT NULL,
                    StartedAt DATETIME NULL,
                    CompletedAt DATETIME NULL
                );
            ";

            using var command = new MySqlCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            Console.WriteLine("Schema created successfully.");
            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create schema: {ex.Message}");
        }
    }

    public bool InsertUser(string username, string passwordHash)
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

            connection.Close();
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

            connection.Close();

            return result?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve user password hash: {ex.Message}");
            return string.Empty;
        }
    }

    public List<Job> GetUserJobs(string userId)
    {
        var jobs = new List<Job>();
        try
        {
            using var connection = GetConnection();
            connection.Open();

            var selectQuery = "SELECT Id, UserId, Type, Status, CreatedAt, StartedAt, CompletedAt FROM Jobs WHERE UserId = @userId;";
            using var command = new MySqlCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@userId", userId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                jobs.Add(new Job
                {
                    Id = Guid.Parse(reader["Id"].ToString() ?? Guid.Empty.ToString()),
                    UserId = reader["UserId"].ToString() ?? string.Empty,
                    Type = reader["Type"].ToString() ?? string.Empty,
                    Status = reader["Status"].ToString() ?? string.Empty,
                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString() ?? DateTime.MinValue.ToString()),
                    StartedAt = reader["StartedAt"] != DBNull.Value ? DateTime.Parse(reader["StartedAt"].ToString() ?? DateTime.MinValue.ToString()) : (DateTime?)null,
                    CompletedAt = reader["CompletedAt"] != DBNull.Value ? DateTime.Parse(reader["CompletedAt"].ToString() ?? DateTime.MinValue.ToString()) : (DateTime?)null
                });
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve user jobs: {ex.Message}");
        }
        return jobs;
    }

    public bool InsertJob(Job job)
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();

            var insertQuery = @"
                INSERT INTO Jobs (Id, UserId, Type, Status, CreatedAt, Payload)
                VALUES (@Id, @UserId, @Type, @Status, @CreatedAt, @Payload);
            ";
            using var command = new MySqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Id", job.Id.ToString());
            command.Parameters.AddWithValue("@UserId", job.UserId);
            command.Parameters.AddWithValue("@Type", job.Type);
            command.Parameters.AddWithValue("@Status", job.Status);
            command.Parameters.AddWithValue("@CreatedAt", job.CreatedAt);
            command.Parameters.AddWithValue("@Payload", "{}"); // Placeholder for actual payload
            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("Job inserted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to insert job: {ex.Message}");
            return false;
        }
    }
}