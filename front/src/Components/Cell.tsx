import {Figure} from "../Entities/Game";

type Props = {
    onPlaceFigure: () => any;
    figure: Figure;
}
export const Cell = (props: Props) => {
    let mappedFigure = undefined;
    switch (props.figure){
        case Figure.X:
            mappedFigure = 'X';
            break;
        case Figure.O:
            mappedFigure = 'O';
            break;
    }
    
    return (
        <div style={{
            width: '100px',
            height: '100px',
            border: '1px solid white',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center'
        }}
        onClick={props.onPlaceFigure}>
            {mappedFigure}
        </div>
    );
}