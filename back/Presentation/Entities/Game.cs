using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Presentation.Entities;

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
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PlayerX { get; set; }

    public Guid PlayerO { get; set; }

    public GameStatus Status { get; set; } = GameStatus.New;

    public Figure C00 { get; set; } = Figure.None;
    public Figure C01 { get; set; } = Figure.None;
    public Figure C02 { get; set; } = Figure.None;
    public Figure C10 { get; set; } = Figure.None;
    public Figure C11 { get; set; } = Figure.None;
    public Figure C12 { get; set; } = Figure.None;
    public Figure C20 { get; set; } = Figure.None;
    public Figure C21 { get; set; } = Figure.None;
    public Figure C22 { get; set; } = Figure.None;

    [NotMapped]
    public Figure this[int f, int s]
    {
        get
        {
            return (f, s) switch
            {
                (0, 0) => C00,
                (0, 1) => C01,
                (0, 2) => C02,
                (1, 0) => C10,
                (1, 1) => C11,
                (1, 2) => C12,
                (2, 0) => C20,
                (2, 1) => C21,
                (2, 2) => C22,
                _ => throw new Exception()
            };
        }
        set
        {
            switch (f, s)
            {
                case (0, 0):
                    C00 = value;
                    break;
                case (0, 1):
                    C01 = value;
                    break;
                case (0, 2):
                    C02 = value;
                    break;
                case (1, 0):
                    C10 = value;
                    break;
                case (1, 1):
                    C11 = value;
                    break;
                case (1, 2):
                    C12 = value;
                    break;
                case (2, 0):
                    C20 = value;
                    break;
                case (2, 1):
                    C21 = value;
                    break;
                case (2, 2):
                    C22 = value;
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}