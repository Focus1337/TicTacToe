import {useRef} from "react";
import {useNavigate} from "react-router-dom";
import axios from "../axios";

export const GetIn = () => {
    const id = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();
    const onGetInExisting = () => {
        if (id.current)
            navigate(`/o/${id.current.value}`);
    }
    const onCreateNew = () => {
        axios.post('http://localhost:81/TicTac/').then(res => navigate(`/x/${res.data}`));
    }

    return (
        <>
            <input ref={id} placeholder={'id'}/>
            <button onClick={onGetInExisting}>Get in</button>
            <button onClick={onCreateNew}>Create new</button>
        </>
    );
}