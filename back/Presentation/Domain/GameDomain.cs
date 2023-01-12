using Presentation.Entities;

namespace Presentation.Domain;

public static class GameDomain
{
    public static Figure WhoseMove(Game game)
    {
        if (IsGameFinished(game))
            return Figure.None;

        var xCount = 0;
        var oCount = 0;
        foreach (var line in game.Cells)
        foreach (var f in line)
            switch (f)
            {
                case Figure.X:
                    xCount++;
                    break;
                case Figure.O:
                    oCount++;
                    break;
                case Figure.None:
                default:
                    break;
            }

        return xCount > oCount ? Figure.O : Figure.X;
    }

    public static Figure IsSomeoneWon(Game game)
    {
        for (var i = 0; i < 3; ++i)
        {
            if (game.Cells[i][0] != Figure.None && game.Cells[i][0] == game.Cells[i][1] &&
                game.Cells[i][0] == game.Cells[i][2])
                return game.Cells[i][0];
            if (game.Cells[0][i] != Figure.None && game.Cells[0][i] == game.Cells[1][i] &&
                game.Cells[0][i] == game.Cells[2][i])
                return game.Cells[0][i];
        }

        if (game.Cells[0][0] != Figure.None && game.Cells[0][0] == game.Cells[1][1] &&
            game.Cells[0][0] == game.Cells[2][2])
            return game.Cells[0][0];

        if (game.Cells[0][2] != Figure.None && game.Cells[0][2] == game.Cells[1][1] &&
            game.Cells[0][2] == game.Cells[2][0])
            return game.Cells[0][2];

        return Figure.None;
    }

    public static bool IsGameFinished(Game game)
    {
        if (IsSomeoneWon(game) != Figure.None)
            return true;
        var emptyCellExists = false;
        foreach (var line in game.Cells)
        foreach (var f in line)
        {
            if (f == Figure.None)
                emptyCellExists = true;
        }

        return !emptyCellExists;
    }
}