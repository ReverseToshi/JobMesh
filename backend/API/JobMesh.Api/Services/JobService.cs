using JobMesh.Api.Data;
using JobMesh.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        try
        {
            // Cache miss - query database
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);

            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[JobService] ❌ Error getting job by id: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Job>> GetUserJobsAsync(string userId)
    {
        try
        {
            var cacheKey = $"user_jobs:{userId}";

            // Cache miss - query database
            var jobs = await _dbContext.Jobs
                .Where(job => job.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[JobService] ❌ Error getting user jobs: {ex.Message}");
            throw;
        }
    }

    public async Task<Job> CreateJobAsync(Job job)
    {
        try
        {
            job.Id = Guid.NewGuid();
            job.Status = "Pending";
            job.CreatedAt = DateTime.UtcNow;

            // 1. Save to database
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"[JobService] ✅ Job saved to database: {job.Id}");

            // 2. Cache the individual job
            var jobCacheKey = $"job:{job.Id}";
            await _redisQueueService.SetAsync(jobCacheKey, job, TimeSpan.FromHours(24));

            // 3. Invalidate user's job list cache so it gets refreshed
            var userJobsCacheKey = $"user_jobs:{job.UserId}";
            await _redisQueueService.DeleteAsync(userJobsCacheKey);
            _logger.LogInformation($"[JobService] Invalidated job list cache for user: {job.UserId}");

            // 4. Enqueue to Redis job queue for worker processing
            await _redisQueueService.EnqueueAsync("job-queue", job.Id.ToString());
            _logger.LogInformation($"[JobService] ✅ Job enqueued to job-queue: {job.Id}");

            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[JobService] ❌ Error creating job: {ex.Message}");
            throw;
        }
    }
}