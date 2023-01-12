using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventBusHandler.Entities;

public enum Figure
{
    None = 0,
    X = 1,
    O = 2,
}

public class Game
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public Figure[][] Cells { get; set; } = null!;

    public static Game CreateNew()
    {
        var game = new Game
        {
            Cells = new Figure[3][]
        };
        for (var i = 0; i < 3; ++i) 
            game.Cells[i] = new Figure[3];
        return game;
    }
}