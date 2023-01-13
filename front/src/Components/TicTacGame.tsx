import {useEffect, useMemo, useState} from "react";
import {useNavigate, useParams} from "react-router-dom";
import {Cell} from "./Cell";
import {Figure, Game, GameStatus} from "../Entities/Game"
import {whoseMove} from "../Domain/gameDomain";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {BASE_URL} from "../config";
import {WhoWon} from "./GameEnd";

export const TicTacGame = () => {
    const {figure, id} = useParams();
    const navigate = useNavigate();

    const [game, setGame] = useState<Game>({
        cells: [[0, 0, 0], [0, 0, 0], [0, 0, 0]],
        id: id || '',
        status: GameStatus.New
    });
    const [connection, setConnection] = useState<null | HubConnection>(null);

    useEffect(() => {
        const jwt = localStorage.getItem('jwt')!;
        const connect = new HubConnectionBuilder()
            .withUrl(BASE_URL + 'game', { accessTokenFactory: () => jwt })
            .withAutomaticReconnect()
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(async () => {
                    connection.on('UpdateGame', (game: Game) => {
                        setGame(game);
                    });
                    connection.on("GameFinish", (winner: Figure) => {
                        navigate(`/gameEnd/${winner === Figure.None ? WhoWon.Tie : winner === (figure === 'x' ? Figure.X : Figure.O) ? WhoWon.Me : WhoWon.Opponent}`)
                    })
                    if (id)
                        connection.invoke(figure === undefined ? "Watch" : "Join", id);
                })
                .catch(error => console.log('Connection failed: ', error));
        }
    }, [connection]);

    const yourMove = whoseMove(game) === figure;

    const onPlaceFigure = useMemo(() =>
        (x: number, y: number, current: Figure) => {
            if (current === Figure.None && yourMove)
                connection?.send("PlaceFigure", x, y, id, figure === 'x' ? 1 : 2).catch(e => console.log(e));
        }, [yourMove, connection]);

    return (
        <>
            {yourMove ? 'Your move' : 'Wait for the opponent\'s move'}
            <br/>
            <br/>
            <div style={{
                display: "grid",
                gridTemplateColumns: "1fr 1fr 1fr",
            }
            }>
                {game.cells.map((arr, x) => arr.map((f, y) => (
                    <Cell key={`${x}${y}`}
                          onPlaceFigure={() => onPlaceFigure(x, y, f)}
                          figure={f}/>)
                ))}
            </div>
        </>
    );
}
