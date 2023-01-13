import {useNavigate} from "react-router-dom";
import {useRef} from "react";

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
        const jwt = 'qwe';
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