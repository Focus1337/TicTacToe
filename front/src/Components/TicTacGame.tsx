import {useEffect, useState} from "react";
import {useNavigate, useParams} from "react-router-dom";
import {Cell} from "./Cell";
import {Figure, Game, GameStatus} from "../Entities/Game"
import {whoseMove} from "../Domain/gameDomain";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {BASE_URL} from "../config";
import axios from "../axios";

export const TicTacGame = () => {
    const {figure, id} = useParams();
    const navigate = useNavigate();

    const [game, setGame] = useState<Game>({
        cells: [[0, 0, 0], [0, 0, 0], [0, 0, 0]],
        id: id || '',
        status: GameStatus.New
    });
    const [winner, setWinner] = useState<Figure>(Figure.None);
    const [connection, setConnection] = useState<null | HubConnection>(null);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl(BASE_URL + 'game')
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
                        console.log(game);
                    });
                    connection.on("GameFinish", (winner: Figure) => {
                        setWinner(winner);
                    })
                    if (id) {
                        const userId = (await axios.get('User/Me')).data;
                        localStorage.setItem('userId', userId);
                        if (figure === undefined)
                            connection.invoke("Watch", id);
                        else
                            connection.invoke("Join", id, userId, (figure === 'x' ? Figure.X : Figure.O));
                    }
                })
                .catch(error => console.log('Connection failed: ', error));
        }
    }, [connection]);

    const yourMove = whoseMove(game) === figure;

    const onPlaceFigure = (x: number, y: number, current: Figure) => {
        if (current === Figure.None && yourMove && game.playerX && game.playerO) {
            const userId = localStorage.getItem('userId');
            connection?.send("PlaceFigure", x, y, id, userId).catch(e => console.log(e));
        }
    }

    const onRestartGame = () => {
        connection?.send('Restart', game.id);
    }

    return (
        <>
            {game.status !== GameStatus.Finished &&
                <p style={{fontSize: '80px'}}>{figure ? `you are playing: ${figure}` : `plays ${whoseMove(game)}`}</p>}
            <br/>
            {figure && game.status !== GameStatus.Finished && (yourMove ? 'Your move' : 'Wait for the opponent\'s move')}
            <br/>
            <br/>
            <div style={{
                display: "grid",
                gridTemplateColumns: "1fr 1fr 1fr",
            }
            }>
                {game.status === GameStatus.Finished
                    ? <p style={{fontSize: '80px'}}>{winner === Figure.None
                        ? 'Tie'
                        : figure
                            ? winner === (figure === 'x' ? Figure.X : Figure.O) ? 'You won' : 'You lost'
                            : `${winner === Figure.X ? 'X' : 'O'} won`}</p>
                    : game.cells.map((arr, x) => arr.map((f, y) => (
                        <Cell key={`${x}${y}`}
                              onPlaceFigure={() => onPlaceFigure(x, y, f)}
                              figure={f}/>)
                    ))}
            </div>
            {figure && game.status === GameStatus.Finished && <button onClick={onRestartGame}>Restart game</button>}
        </>
    );
}
