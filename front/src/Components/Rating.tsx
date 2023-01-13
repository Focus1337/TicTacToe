import {useEffect, useState} from "react";
import axios from "../axios";

export const Rating = () => {
    const [users, setUsers] = useState<{ userName: string, rating: number }[]>([]);

    useEffect(() => {
        axios.get('User/rating').then(res => setUsers(res.data));
    }, []);

    return (<>{
        users.map(u => (
            <div key={u.userName} style={{display: 'flex'}}>
                <p style={{width: '300px'}}>{u.userName}</p>
                <p style={{width: '50px'}}>{u.rating}</p>
            </div>))}
    </>)
}