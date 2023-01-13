import {useNavigate} from "react-router-dom";
import {useRef} from "react";
import axios from "../axios";

export const Login = () => {
    const navigate = useNavigate();

    const username = useRef<HTMLInputElement>(null);
    const pass = useRef<HTMLInputElement>(null);

    const onLogin = async () => {
        const usernameData = username.current?.value;
        const pass1Data = pass.current?.value;
        if(!usernameData || !pass1Data)
        {
            alert('fill all fields');
            return;
        }
        if(usernameData.length < 6)
        {
            alert('username should be at least 6 symbols length');
            return;
        }
        const res = await axios.post('Auth/Login', {
            username: usernameData,
            password: pass1Data,
            grant_type: 'password'
        }, {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            }
        });
        console.log(res.data)
        const jwt = res.data.access_token;
        localStorage.setItem('jwt', jwt);
        window.location.replace('/');
    }
    
    const onGoToRegister = () => {
        navigate('/register');
    }

    return (
        <>
            <input ref={username} placeholder={'username'}/>
            <input ref={pass} placeholder={'password'}/>
            <button onClick={onLogin}>login</button>
            <br/>
            <button onClick={onGoToRegister}>register</button>
        </>
    )
}