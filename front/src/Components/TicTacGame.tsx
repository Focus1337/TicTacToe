import React, {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {Cell} from "./Cell";
import {Figure, Game, GameStatus} from "../Entities/Game"
import {whoseMove} from "../Domain/gameDomain";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {BASE_URL} from "../config";
import axios from "../axios";
import IMessage from "../chat/entities/IMessage";
import {v4 as uuidv4} from "uuid";
import ChatWindow from "../chat/Chat/ChatWindow/ChatWindow";
import ChatInput from "../chat/Chat/ChatInput/ChatInput";
import chatAxios from "../chat/chatAxios";

export const TicTacGame = () => {
    const {figure, id} = useParams();

    const [game, setGame] = useState<Game>({
        cells: [[0, 0, 0], [0, 0, 0], [0, 0, 0]],
        id: id || '',
        status: GameStatus.New,
        maxRating: 0,
        createdDateTime: '',
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
                        const meWin = winner === (figure === 'x' ? Figure.X : figure === 'o' ? Figure.O : null);
                        if (meWin)
                            sendMessage(`[win] ${userName} wins`)
                    })
                    if (id) {
                        const userId = localStorage.getItem('userId');
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


    const [chat, setChat] = useState<IMessage[]>([]);
    const [chatConnection, setChatConnection] = useState<null | HubConnection>(null);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl('http://localhost:82/' + 'chat')
            .withAutomaticReconnect()
            .build();

        setChatConnection(connect);
    }, []);

    useEffect(() => {
        if (chatConnection) {
            chatConnection
                .start()
                .then(async () => {
                    chatConnection.on('ReceiveMessage', (message: IMessage) => {
                        if (message.gameId === game.id)
                            setChat(prev => [...prev, message]);
                    });
                })
                .catch(error => console.log('Connection failed: ', error));
        }
    }, [chatConnection]);

    useEffect(() => {
        chatAxios.get<IMessage[]>(`api/messages?gameId=${game.id}`).then(res => {
            console.log(res.data)
            setChat(res.data)
        });
    }, [])

    const userName = localStorage.getItem('userName');

    const sendMessage = async (text: string) => {
        const chatMessage: IMessage = {
            id: uuidv4(),
            userName: userName || '',
            text: text,
            dateTime: new Date(),
            gameId: game.id,
        };

        if (chatConnection)
            await chatConnection
                .send("SendMessage", chatMessage)
                .catch(() => console.log('Publishing in SignalR failed'));
    }


    return (
        <div style={{display: 'flex', height: '97vh'}}>
            <div>
                {game.status !== GameStatus.Finished &&
                    <p style={{fontSize: '80px'}}>{figure ? `you are playing: ${figure}` : `plays ${whoseMove(game)}`}</p>}
                <br/>
                {figure && game.status !== GameStatus.Finished && (yourMove ? 'Your move' : 'Wait for the opponent\'s move')}
                <br/>
                <br/>
                <div style={{
                    display: "grid",
                    gridTemplateColumns: "1fr 1fr 1fr",
                    width: '300px'
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
            </div>
            <>
                <div className="flex flex-col flex-grow w-full max-w-xl bg-white shadow-xl rounded-lg overflow-hidden"
                     style={{color: 'black'}}>
                    <ChatWindow chat={chat}/>
                    <hr/>
                    <ChatInput sendMessage={sendMessage}/>
                </div>
            </>
        </div>
    );
}
