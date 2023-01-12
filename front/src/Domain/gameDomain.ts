import {Figure, Game} from "../Entities/Game"

export const whoseMove = (game: Game) => {
    let xCount = 0;
    let oCount = 0;
    game.cells.forEach(line => line.forEach(f => {
        if (f === Figure.X)
            xCount++;
        if (f === Figure.O)
            oCount++;
    }));

    return xCount > oCount ? 'o' : 'x';
}