using StackExchange.Redis;

namespace redis_dotnet.Services;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;

    public RedisService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");

        _redis = ConnectionMultiplexer.Connect(connectionString);
    }

    public async Task<string> PingAsync()
    {
        var db = _redis.GetDatabase();

        var result = await db.PingAsync();

        return $"Redis connected ({result.TotalMilliseconds} ms)";
    }

    public void setString(string key, string value, TimeSpan expiry = default(TimeSpan))
    {
        var db = _redis.GetDatabase();

        db.StringSet(key, value, expiry);
    }

    public string? getString(string key)
    {
        var db = _redis.GetDatabase();

        var val = db.StringGet(key);
        return val.IsNull ? null : val.ToString();
    }

    public bool deleteKey(string key)
    {
        var db = _redis.GetDatabase();

        return db.KeyDelete(key);
    }

    public bool keyExists(string key)
    {
        var db = _redis.GetDatabase();

        return db.KeyExists(key);
    }

    public TimeSpan? getKeyTTL(string key)
    {
        var db = _redis.GetDatabase();

        return db.KeyTimeToLive(key);
    }

    #region Hash
    public void setHash(string key, HashEntry[] hashEntries, TimeSpan expiry = default(TimeSpan))
    {
        var db = _redis.GetDatabase();

        db.HashSet(key, hashEntries);
        if (expiry != default(TimeSpan))
        {
            db.KeyExpire(key, expiry);
        }
    }

    public HashEntry[] getHash(string key)
    {
        var db = _redis.GetDatabase();

        return db.HashGetAll(key);
    }

    public HashEntry? getHashField(string key, string field)
    {
        var db = _redis.GetDatabase();

        var value = db.HashGet(key, field);
        if (value.IsNull)
        {
            return null;
        }
        return new HashEntry(field, value);
    }

    public bool deleteHashField(string key, string field)
    {
        var db = _redis.GetDatabase();

        return db.HashDelete(key, field);
    }
    #endregion

    #region List as a Queue
    public long addJob(string key, string value)
    {
        var db = _redis.GetDatabase();
        return db.ListLeftPush(key, value);
    }

    public string? getJob(string key)
    {
        var db = _redis.GetDatabase();
        return db.ListRightPop(key);
    }
    #endregion

}