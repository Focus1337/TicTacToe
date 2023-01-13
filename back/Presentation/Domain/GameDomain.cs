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
        for (var f = 0; f < 3; ++f)
        for (var s = 0; s < 3; ++s)
            switch (game[f, s])
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
            if (game[i, 0] != Figure.None && game[i, 0] == game[i, 1] &&
                game[i, 0] == game[i, 2])
                return game[i, 0];
            if (game[0, i] != Figure.None && game[0, i] == game[1, i] &&
                game[0, i] == game[2, i])
                return game[0, i];
        }

        if (game[0, 0] != Figure.None && game[0, 0] == game[1, 1] &&
            game[0, 0] == game[2, 2])
            return game[0, 0];

        if (game[0, 2] != Figure.None && game[0, 2] == game[1, 1] &&
            game[0, 2] == game[2, 0])
            return game[0, 2];

        return Figure.None;
    }

    public static bool IsGameFinished(Game game)
    {
        if (IsSomeoneWon(game) != Figure.None)
            return true;
        var emptyCellExists = false;
        for (var f = 0; f < 3; ++f)
        for (var s = 0; s < 3; ++s)
        {
            if (game[f, s] == Figure.None)
                emptyCellExists = true;
        }

        return !emptyCellExists;
    }
}