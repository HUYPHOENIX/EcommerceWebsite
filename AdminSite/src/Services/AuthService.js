import axios from 'axios';
import {jwtDecode} from 'jwt-decode'


const API_URL = 'http://localhost:5007/api/Auth';


export const login = async (Email, Password) => {

    try {

        const response = await axios.post(
            `${API_URL}/admin-login`,
            { Email, Password }
        );

        if (response.data.accessToken) {

            localStorage.setItem(
                'accessToken',
                response.data.accessToken
            );
        }

        return {
            success: true,
            data: response.data
        };

    } catch (error) {

        return {
            success: false,
            error:
                error.response?.data?.message ||
                "Đăng nhập thất bại"
        };
    }
};

export const logout = () => {
    localStorage.removeItem('accessToken');
};

export const getToken = () => {
    return localStorage.getItem("accessToken");
};

export const isTokenExpired = (token, bufferTime = 60) => {
    try {
        if (!token) return true;
        const decodedToken = jwtDecode(token);
        if (!decodedToken.exp) return true;
        const currentTime = Date.now() / 1000;
        const expirationWithBuffer = decodedToken.exp - bufferTime;
        return expirationWithBuffer < currentTime;

    } catch (error) {
        console.error('Invalid token:', error);
        return true;
    }
};

export const isAuthenticated = () => {
    const token = getToken();
    if (!token) return false;

    if (isTokenExpired(token)) {
        logout(); 
        return false;
    }
    return true;
};