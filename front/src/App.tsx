import React from 'react';
import './App.css';
import {BrowserRouter, Link, Navigate, Route, Routes} from 'react-router-dom';
import {GameEnd, GetIn, TicTacGame} from './Components';
import {Register} from "./Components/Register";
import {Login} from "./Components/Login";
import {GamesList} from "./Components/GamesList";
import axios from "./axios";
import {Rating} from "./Components/Rating";

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
                    {authorized ? (<div style={{display: 'flex', flexDirection: 'row', width: '100%', justifyContent: 'center'}}>
                            <button onClick={onSignOut} style={{marginRight: '20px'}}>Sign out</button>
                            <Link to={'/list'} style={{color: "white", marginRight: '20px'}}>Games list</Link>
                            <button onClick={onCreateNew} style={{marginRight: '20px'}}>Create new game</button>
                            <Link to={'/rating'} style={{color: "white", marginRight: '20px'}}>Rating</Link>
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
                        <Route path={'/rating'} element={<Rating/>}/>
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
