import {useRef, useState} from "react";
import InfiniteScroll from "react-infinite-scroller";
import axios from "../axios";
import {Game, GameStatus} from "../Entities/Game";
import {useNavigate} from "react-router-dom";

export const GamesList = () => {
    const navigate = useNavigate();
    const [games, setGames] = useState<Game[]>([]);
    const [hasMore, setHasMore] = useState(true);
    const page = useRef(-1);

    const onLoadMore = async () => {
        page.current += 1;
        await axios.get(`/TicTac?page=${page.current}`)
            .then(res => {
                setGames(prev => [...prev, ...res.data.games]);
                setHasMore(res.data.hasMore)
            });
    }

    const onWatch = (id: string) => {
        navigate(`/${id}`);
    }
    const onJoin = (game: Game) => {
        if (!game.playerX || !game.playerO)
            navigate(`/${game.playerX ? 'o' : 'x'}/${game.id}`);
    }

    return (
        <InfiniteScroll pageStart={0} loadMore={onLoadMore} hasMore={hasMore}>
            {games?.map(g => <div key={g.id} style={{display: 'flex'}}>
                <p style={{width: '300px'}}>{g.id}</p>
                <button onClick={() => onWatch(g.id)}>Watch</button>
                {g.status === GameStatus.New && <button onClick={() => onJoin(g)}>Join</button>}
            </div>)}
        </InfiniteScroll>
    )
}
