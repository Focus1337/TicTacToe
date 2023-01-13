import React from 'react';
import './App.css';
import Chat from "./Chat/Chat";

function App() {
    return (
        <div
            className="flex flex-col items-center justify-center w-screen min-h-screen bg-gray-100 text-gray-800 p-10">
            <Chat/>
        </div>
    );
}

export default App;
