export enum Figure {
    None = 0,
    X = 1,
    O = 2,
}

export interface Game
{
    id: string;
    cells: Figure[][];
}