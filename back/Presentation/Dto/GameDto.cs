﻿using Presentation.Entities;

namespace Presentation.Dto;

public class GameDto
{
    public Guid Id { get; set; }
    public Figure[][] Cells { get; set; }
    public GameStatus Status { get; set; }

    public GameDto(Game game)
    {
        this.Id = game.Id;
        this.Status = game.Status;
        this.Cells = new Figure[3][];
        for (var f = 0; f < 3; ++f)
            this.Cells[f] = new Figure[3];
        for (var f = 0; f < 3; ++f)
        for (var s = 0; s < 3; ++s)
            this.Cells[f][s] = game[f, s];
    }
}