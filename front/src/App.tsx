import React from 'react';
import './App.css';
import {BrowserRouter, Navigate, Route, Routes} from 'react-router-dom';
import {GameEnd, GetIn, TicTacGame} from './Components';
import {Register} from "./Components/Register";
import {Login} from "./Components/Login";

function App() {
    const onSignOut = () => {
        localStorage.removeItem('jwt');
    }

    return (
        <div className="App">
            <div style={{justifySelf: 'self-start'}}>
                <button onClick={onSignOut}>Sign out</button>
            </div>
            <div className="Header">
                    <BrowserRouter>
                        <Routes>
                            <Route path={'/'} element={<Navigate replace to={'/getIn'}/>}/>
                            <Route path={'/register'} element={<Register/>}/>
                            <Route path={'/login'} element={<Login/>}/>
                            <Route path={'/getIn'} element={<GetIn/>}/>
                            <Route path={'/gameEnd/:winner'} element={<GameEnd/>}/>
                            <Route path={"/:figure/:id"} element={<TicTacGame/>}/>
                        </Routes>
                    </BrowserRouter>
            </div>
        </div>
    );
}

export default App;
