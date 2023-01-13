import React from 'react';
import './App.css';
import {BrowserRouter, Link, Navigate, Route, Routes} from 'react-router-dom';
import {GameEnd, GetIn, TicTacGame} from './Components';
import {Register} from "./Components/Register";
import {Login} from "./Components/Login";
import {GamesList} from "./Components/GamesList";
import axios from "./axios";

function App() {
    const onSignOut = () => {
        localStorage.removeItem('jwt');
        window.location.replace('/login');
    }

    const onGoToLogin = () => {
        window.location.replace('/login');
    }

    const authorized = !!localStorage.getItem('jwt');
    const onCreateNew = () => {
        axios.post('http://localhost:81/TicTac/').then(res => window.location.replace(`/x/${res.data}`));
    }

    return (
        <div className="App">
            <BrowserRouter>
                <div style={{justifySelf: 'self-start'}}>
                    {authorized ? (<div style={{display: 'flex', flexDirection: 'row'}}>
                            <button onClick={onSignOut} style={{marginRight: '10px'}}>Sign out</button>
                            <Link to={'/list'} style={{color: "white", marginRight: '10px'}}>Games list</Link>
                            <button onClick={onCreateNew}>Create new game</button>
                        </div>)
                        : (
                            <button onClick={onGoToLogin}>login</button>
                        )
                    }
                </div>
                <div className="Header">
                    <Routes>
                        <Route path={'/'} element={<Navigate replace to={'/getIn'}/>}/>
                        <Route path={'/register'} element={<Register/>}/>
                        <Route path={'/login'} element={<Login/>}/>
                        <Route path={'/list'} element={<GamesList/>}/>
                        <Route path={'/getIn'} element={<GetIn/>}/>
                        <Route path={'/gameEnd/:winner'} element={<GameEnd/>}/>
                        <Route path={"/:figure/:id"} element={<TicTacGame/>}/>
                        <Route path={"/:id"} element={<TicTacGame/>}/>
                    </Routes>
                </div>
            </BrowserRouter>
        </div>
    );
}

export default App;
