using MongoDB.Driver;

namespace redis_dotnet.Services;


public class MongoService
{
    private readonly IMongoClient _client;
    public MongoService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");

        _client = new MongoClient(connectionString);
    }

    public string GetDatabaseName()
    {
        var database = _client.GetDatabase("mongo");

        return database.DatabaseNamespace.DatabaseName;
    }
}