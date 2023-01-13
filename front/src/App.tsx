import React from 'react';
import './App.css';
import {BrowserRouter, Navigate, Route, Routes} from 'react-router-dom';
import {GameEnd, GetIn, TicTacGame} from './Components';
import {Register} from "./Components/Register";
import {Login} from "./Components/Login";
import {GamesList} from "./Components/GamesList";

function App() {
    const onSignOut = () => {
        localStorage.removeItem('jwt');
        window.location.replace('/login')
    }

    const onGoToLogin = () => {
        window.location.replace('/login')
    }

    const authorized = !!localStorage.getItem('jwt');

    return (
        <div className="App">
            <div style={{justifySelf: 'self-start'}}>
                {authorized ? (
                        <button onClick={onSignOut}>Sign out</button>
                    )
                    : (
                        <button onClick={onGoToLogin}>login</button>
                    )
                }
            </div>
            <div className="Header">
                <BrowserRouter>
                    <Routes>
                        <Route path={'/'} element={<Navigate replace to={'/getIn'}/>}/>
                        <Route path={'/register'} element={<Register/>}/>
                        <Route path={'/login'} element={<Login/>}/>
                        <Route path={'/list'} element={<GamesList/>}/>
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
