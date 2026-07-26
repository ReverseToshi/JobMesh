using JobMesh.Api.Data;
using JobMesh.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobMesh.Api.Services;

public class JobService
{
    private readonly AppDbContext _dbContext;

    public JobService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Job>> GetUserJobsAsync(string userId)
    {
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