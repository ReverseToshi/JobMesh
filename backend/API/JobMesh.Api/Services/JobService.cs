using JobMesh.Api.Data;
using JobMesh.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobMesh.Api.Services;

public class JobService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<JobService> _logger;

    private readonly RedisQueueService _redisQueueService;

    public JobService(AppDbContext dbContext, RedisQueueService redisQueueService, ILogger<JobService> logger)
    {
        _dbContext = dbContext;
        _redisQueueService = redisQueueService;
        _logger = logger;
    }
    public async Task<Job?> GetJobByIdAsync(Guid jobId)
    {
        return await _dbContext.Jobs.FirstOrDefaultAsync(job => job.Id == jobId);
    }

    public async Task<List<Job>> GetUserJobsAsync(string userId)
    {
        var cachedJobs = await _redisQueueService.DequeueAsync($"user_jobs:{userId}");
        if (cachedJobs != null)
        {
            _logger.LogInformation($"[JobService] Retrieved jobs for user '{userId}' from Redis cache.");
            // Deserialize cachedJobs to List<Job> if needed
            return new List<Job>(); // Placeholder, implement deserialization if necessary
        }

        return await _dbContext.Jobs
            .Where(job => job.UserId == userId)
            .ToListAsync();
    }

    public async Task<Job> CreateJobAsync(Job job)
    {
        job.Id = Guid.NewGuid();
        job.Status = "Pending";
        job.CreatedAt = DateTime.UtcNow;

        _dbContext.Jobs.Add(job);
        await _dbContext.SaveChangesAsync();

        return job;
    }
}