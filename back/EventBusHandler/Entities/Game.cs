namespace EventBusHandler.Entities;

public enum Figure
{
    None = 0,
    X = 1,
    O = 2,
}

public enum GameStatus
{
    New = 0,
    Started = 1,
    Finished = 2,
}

public class Game
{
    public Guid Id { get; set; }

    public Guid? PlayerX { get; set; }

    public Guid? PlayerO { get; set; }

    public GameStatus Status { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public Figure C00 { get; set; } 
    public Figure C01 { get; set; }
    public Figure C02 { get; set; }
    public Figure C10 { get; set; }
    public Figure C11 { get; set; } 
    public Figure C12 { get; set; }
    public Figure C20 { get; set; }
    public Figure C21 { get; set; }
    public Figure C22 { get; set; }
}