import React from 'react';
import './App.css';
import {BrowserRouter, Route, Routes} from 'react-router-dom';
import {GameEnd, GetIn, TicTacGame} from './Components';

function App() {
    return (
        <div className="App">
            <div className="Header">
                <BrowserRouter>
                    <Routes>
                        <Route path={'/'} element={<GetIn/>}/>
                        <Route path={'/gameEnd/:winner'} element={<GameEnd/>}/>
                        <Route path={"/:figure/:id"} element={<TicTacGame/>}/>
                    </Routes>
                </BrowserRouter>
            </div>
        </div>
    );
}

export default App;
