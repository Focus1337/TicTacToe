import axios from "axios";
import {BASE_URL} from "../config";

const jwt = localStorage.getItem('jwt');

const instance = axios.create({
    baseURL: BASE_URL,
    headers: {authorization: `Bearer ${jwt}`}
});

export default instance;