﻿import {useRef, useState} from "react";
import InfiniteScroll from "react-infinite-scroller";
import axios from "../axios";
import {Game, GameStatus} from "../Entities/Game";

export const GamesList = () => {
    const [games, setGames] = useState<Game[]>([]);
    const [hasMore, setHasMore] = useState(true);
    const page = useRef(-1);

    const onLoadMore = async () => {
        page.current += 1;
        await axios.get(`/TicTac?page=${page.current}`)
            .then(res => {
                setGames([...games, ...res.data.games]);
                setHasMore(res.data.hasMore)
            });
    }

    return (
        <InfiniteScroll pageStart={0} loadMore={onLoadMore} hasMore={hasMore}>
            {games?.map(g => <div key={g.id} style={{display: 'flex'}}>
                <p>{g.id}</p>
                <button>Watch</button>
                {g.status === GameStatus.New && <button>Join</button>}
            </div>)}
        </InfiniteScroll>
    )
}