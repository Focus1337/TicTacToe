import axios from "../axios";
import {useRef} from "react";
import {useNavigate} from "react-router-dom";

export const CreateGame = () => {
    const maxRating = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();
    const onCreateNew = () => {
        if (!maxRating.current || !maxRating.current.value) {
            alert('Fill max rating');
            return;
        }
        axios.post('http://localhost:81/TicTac/', {maxRating: maxRating.current.value}).then(res => navigate(`/x/${res.data}`));
    }

    return (<>
        <input ref={maxRating} placeholder={'Max rating'}/>
        <button onClick={onCreateNew}>Create and join</button>
    </>)
}