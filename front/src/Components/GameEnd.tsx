import {useNavigate, useParams} from "react-router-dom";

export enum WhoWon {
    Me = 'meWon',
    Opponent = 'meLose',
    Tie = 'tie',
}

export const GameEnd = () => {
    const {winner} = useParams();
    const navigate = useNavigate();
    let won;
    switch (winner) {
        case WhoWon.Me:
            won = <p>You won!</p>;
            break;
        case WhoWon.Opponent:
            won = <p>You lost</p>;
            break;
        case WhoWon.Tie:
        default:
            won = <p>Tie</p>;
            break;
    }
    
    const onGoBack = () => {
        navigate('/');
    }
    
    return (
        <>
            {won}
            <p onClick={onGoBack}>Go back</p>
        </>
    )
}