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

    const userId = localStorage.getItem('userId');
    const onGoBack = (game: Game) => {
        const figure = game.playerX === userId ? 'x' : 'o';
        navigate(`/${figure}/${game.id}`);
    }

    const userRating = Number.parseInt(localStorage.getItem('userRating') || '0');

    return (
        <InfiniteScroll pageStart={0} loadMore={onLoadMore} hasMore={hasMore}>
            {games?.map(g => {
                const isParticipant = g.playerX === userId || g.playerO == userId;

                const date = g.createdDateTime;
                const formattedDate = date.slice(0, date.indexOf('.')).replace('T', ' ');

                return <div key={g.id} style={{display: 'flex', marginBottom: '20px'}}>
                    <p style={{width: '400px', margin: '0'}}>{g.id}</p>
                    <p style={{width: '200px', margin: '0'}}>{g.maxRating}</p>
                    <p style={{width: '200px', margin: '0'}}>{g.creatorName}</p>
                    <p style={{width: '200px', margin: '0'}}>{formattedDate}</p>
                    <button onClick={() => onWatch(g.id)}
                            disabled={isParticipant}
                            style={{marginRight: '30px', color: isParticipant ? 'grey' : 'white'}}>
                        Watch
                    </button>
                    <button onClick={() => onJoin(g)}
                            disabled={g.status !== GameStatus.New || g.maxRating < userRating || isParticipant}
                            style={{
                                marginRight: '30px',
                                color: (g.status !== GameStatus.New || g.maxRating < userRating || isParticipant) ? 'grey' : 'white'
                            }}>
                        Join
                    </button>
                    <button onClick={() => onGoBack(g)}
                            disabled={!isParticipant}
                            style={{marginRight: '30px', color: !isParticipant ? 'grey' : 'white'}}>
                        Go back to
                    </button>
                </div>
            })}
        </InfiniteScroll>
    )
}
