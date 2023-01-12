using EventBusHandler.Entities;
using MongoDB.Driver;

namespace EventBusHandler.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext()
    {
        var mongoClient = new MongoClient("mongodb://mongo:27017");
        _db = mongoClient.GetDatabase("tictac");
    }

    public IMongoCollection<Game> GetGameCollection() => _db.GetCollection<Game>("games");
}