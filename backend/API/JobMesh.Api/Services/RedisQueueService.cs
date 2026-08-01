using StackExchange.Redis;

namespace JobMesh.Api.Services;

public class RedisQueueService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisQueueService> _logger;

    public RedisQueueService(IConnectionMultiplexer redis, ILogger<RedisQueueService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task EnqueueAsync(string queueName, string jobId)
    {
        try
        {
            if (_redis == null)
            {
                _logger.LogError("[Redis] Connection multiplexer is null");
                throw new InvalidOperationException("Redis connection is not available");
            }

            if (!_redis.IsConnected)
            {
                _logger.LogError("[Redis] Not connected to Redis server");
                throw new InvalidOperationException("Redis connection is not established");
            }

            var database = _redis.GetDatabase();
            var result = await database.ListRightPushAsync(queueName, jobId);

            _logger.LogInformation($"[Redis] ✅ Job '{jobId}' enqueued to '{queueName}' (Queue length: {result})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error enqueuing job '{jobId}': {ex.Message}");
            throw;
        }
    }

    public async Task<string?> DequeueAsync(string queueName)
    {
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                _logger.LogError("[Redis] Connection not available for dequeue");
                return null;
            }

            var database = _redis.GetDatabase();
            var value = await database.ListLeftPopAsync(queueName);

            if (value.HasValue)
            {
                _logger.LogInformation($"[Redis] ✅ Dequeued from '{queueName}': {value}");
                return value.ToString();
            }

            _logger.LogDebug($"[Redis] Queue '{queueName}' is empty");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error dequeuing from '{queueName}': {ex.Message}");
            return null;
        }
    }

    public async Task<long> GetQueueLengthAsync(string queueName)
    {
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                _logger.LogError("[Redis] Connection not available");
                return -1;
            }

            var database = _redis.GetDatabase();
            var length = await database.ListLengthAsync(queueName);

            _logger.LogInformation($"[Redis] Queue '{queueName}' length: {length}");
            return length;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error getting queue length: {ex.Message}");
            return -1;
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                _logger.LogError("[Redis] Connection not available for GetAsync");
                return default;
            }

            var database = _redis.GetDatabase();
            var value = await database.StringGetAsync(key);

            if (!value.HasValue)
            {
                _logger.LogDebug($"[Redis] Key '{key}' not found in cache");
                return default;
            }

            var deserializedValue = System.Text.Json.JsonSerializer.Deserialize<T>(value);
            _logger.LogInformation($"[Redis] ✅ Retrieved key '{key}' from cache");
            return deserializedValue;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error getting key '{key}': {ex.Message}");
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                _logger.LogError("[Redis] Connection not available for SetAsync");
                return;
            }

            var database = _redis.GetDatabase();
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, serializedValue, expiry.GetValueOrDefault(TimeSpan.FromHours(24)));

            _logger.LogInformation($"[Redis] ✅ Set key '{key}' in cache with expiry: {expiry}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error setting key '{key}': {ex.Message}");
        }
    }

    public async Task DeleteAsync(string key)
    {
        try
        {
            if (_redis == null || !_redis.IsConnected)
            {
                _logger.LogError("[Redis] Connection not available for DeleteAsync");
                return;
            }

            var database = _redis.GetDatabase();
            var deleted = await database.KeyDeleteAsync(key);

            if (deleted)
            {
                _logger.LogInformation($"[Redis] ✅ Deleted key '{key}' from cache");
            }
            else
            {
                _logger.LogDebug($"[Redis] Key '{key}' not found in cache for deletion");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Redis] ❌ Error deleting key '{key}': {ex.Message}");
        }
    }
}