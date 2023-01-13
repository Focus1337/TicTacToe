import {useEffect, useRef, useState} from "react";
import axios from "../axios";
import {Game} from "../Entities/Game";

export const GamesList = () => {
    const [games, setGames] = useState<Game[]>();
    const page = useRef(0);
    
    useEffect(() => {
        axios.get(`/TicTac?page=${page.current}`)
            .then(res => setGames(res.data));
    }, [])
    
    return (<>
        {games?.map(g => <p>{g.id}</p>)}
        </>)
}
