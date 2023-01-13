import React, {useState, useEffect} from 'react';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {v4 as uuidv4} from 'uuid';
import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput/ChatInput";
import IMessage from "../entities/IMessage";
import axios from "../axios";
import {Button} from "@mui/material";
import {BASE_URL} from "../config";

export default function Chat() {
    const [chat, setChat] = useState<IMessage[]>([]);
    const [connection, setConnection] = useState<null | HubConnection>(null);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl(BASE_URL + 'chat')
            .withAutomaticReconnect()
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(async () => {
                    connection.on('ReceiveMessage', (message: IMessage) => {
                        setChat(prev => [...prev, message]);
                    });
                })
                .catch(error => console.log('Connection failed: ', error));
        }
    }, [connection]);

    useEffect(() => {
        axios.get<IMessage[]>('api/messages').then(res => setChat(res.data));
    }, [])

    const sendMessage = async (userName: string, text: string) => {
        const chatMessage: IMessage = {
            id: uuidv4(),
            userName: userName,
            text: text,
            dateTime: new Date()
        };

        if (connection)
            await connection
                .send("SendMessage", chatMessage)
                .catch(() => console.log('Publishing in SignalR failed'));
    }

    return (
        <div className="flex flex-col flex-grow w-full max-w-xl bg-white shadow-xl rounded-lg overflow-hidden">
            <Button onClick={async () => {
                await axios.delete('api/messages');
                setChat([]);
            }}>
                Reset
            </Button>
            <ChatWindow chat={chat}/>
            <hr/>
            <ChatInput sendMessage={sendMessage}/>
        </div>
    );
}