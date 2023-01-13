import React, {useState} from 'react';
import {Input, InputLabel} from "@mui/material";

interface ChatInputProps {
    sendMessage: (user: string, message: string) => void;
}

export default function ChatInput({sendMessage}: ChatInputProps) {
    const [user, setUser] = useState('');
    const [message, setMessage] = useState('');

    const submitHandler = (event: React.FormEvent) => {
        event.preventDefault();

        const isUserProvided = user && user !== '';
        const isMessageProvided = message && message !== '';

        if (isUserProvided && isMessageProvided) {
            sendMessage(user, message);
        } else {
            alert('Please insert an user and a message.');
        }
    }

    const userUpdateHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUser(event.target.value);
    }

    const messageUpdateHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setMessage(event.target.value);
    }

    return (
        <form onSubmit={submitHandler} className="bg-gray-300 p-1">
            <input
                id="user"
                name="user"
                value={user}
                onChange={userUpdateHandler}
                className="flex items-center h-7 w-full rounded px-2 text-sm mb-1"
                placeholder="username"
            />
            <input
                type="text"
                id="message"
                name="message"
                value={message}
                onChange={messageUpdateHandler}
                className="flex items-center h-7 w-full rounded px-2 text-sm"
                placeholder="message"
            />
            <Input type="Submit" value="Submit" className="invisible"></Input>
        </form>
    )
}
