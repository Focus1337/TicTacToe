import React, {useState} from 'react';

interface ChatInputProps {
    sendMessage: (message: string) => void;
}

export default function ChatInput({sendMessage}: ChatInputProps) {
    const [message, setMessage] = useState('');

    const submitHandler = (event: React.FormEvent) => {
        event.preventDefault();

        const isMessageProvided = message && message !== '';

        if (isMessageProvided) {
            sendMessage(message);
        } else {
            alert('Please insert an user and a message.');
        }
    }

    const messageUpdateHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setMessage(event.target.value);
    }

    return (
        <form onSubmit={submitHandler} className="bg-gray-300 p-1">
            <input
                type="text"
                id="message"
                name="message"
                value={message}
                onChange={messageUpdateHandler}
                className="flex items-center h-7 w-full rounded px-2 text-sm"
                placeholder="message"
            />
            <input type="Submit" value="Submit" className="invisible"></input>
        </form>
    )
}
