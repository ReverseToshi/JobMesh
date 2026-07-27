using JobMesh.Api.Data;
using JobMesh.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace JobMesh.Api.Services;

public class UserService
{
    private readonly AppDbContext _dbContext;

    private readonly RedisQueueService _redisQueueService;

    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext dbContext, RedisQueueService redisQueueService, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _redisQueueService = redisQueueService;
        _logger = logger;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var cachedUser = await _redisQueueService.DequeueAsync($"user:{username}");
        if (cachedUser != null)
        {
            _logger.LogInformation($"[UserService] Retrieved user '{username}' from Redis cache.");
            return new User { Username = username, PasswordHash = cachedUser };
        }

        _logger.LogInformation($"[UserService] User '{username}' not found in Redis cache. Querying database...");


        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user != null)
        {
            await _redisQueueService.EnqueueAsync($"user:{username}", user.PasswordHash);
            _logger.LogInformation($"[UserService] Cached user '{username}' in Redis.");
        }

        return user;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.Id = Guid.NewGuid().ToString();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        await _redisQueueService.EnqueueAsync($"user:{user.Username}", user.PasswordHash);
        _logger.LogInformation($"[UserService] Created and cached new user '{user.Username}' in Redis.");
        
        return user;
    }
}