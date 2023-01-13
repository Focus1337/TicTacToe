using Presentation.Entities;

namespace Presentation.Dto;

public class GameDto
{
    public Guid Id { get; set; }
    public Figure[][] Cells { get; set; }
    public GameStatus Status { get; set; }
    public string? CreatorName { get; set; }
    public int MaxRating { get; set; }
    public Guid? PlayerX { get; set; }
    public Guid? PlayerO { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public GameDto(Game game)
    {
        this.Id = game.Id;
        this.Status = game.Status;
        this.PlayerX = game.PlayerX;
        this.PlayerO = game.PlayerO;
        this.CreatorName = game.CreatorName;
        this.MaxRating = game.MaxRating;
        this.CreatedDateTime = game.CreatedDateTime;

        this.Cells = new Figure[3][];
        for (var f = 0; f < 3; ++f)
            this.Cells[f] = new Figure[3];
        for (var f = 0; f < 3; ++f)
        for (var s = 0; s < 3; ++s)
            this.Cells[f][s] = game[f, s];
    }
}