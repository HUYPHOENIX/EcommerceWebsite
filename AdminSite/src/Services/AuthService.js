import axios from 'axios';
// import { isTokenExpired } from "../Utils/TokenUtils";


const API_URL = 'http://localhost:5007/api/Auth';

export const login = async (Email, Password) => {
    try {
        const response = await axios.post(
            `${API_URL}/admin-login`,
            { Email, Password }
        );
        
        if (response.data.accessToken) {
            localStorage.setItem('accessToken', response.data.accessToken);
        }

        return {
            success: true,
            data: response.data
        };

    } catch (error) {
        let errorMessage = "Không thể kết nối đến server";

        if (error.response?.status === 401) {
            errorMessage = "Tài khoản không tồn tại hoặc sai mật khẩu";
        } else if (error.response?.status === 403) {
            errorMessage = "Bạn không có quyền truy cập";
        }
        return {
            success: false,
            error: errorMessage
        };
    }
};

export const logout = () => {
    localStorage.removeItem('accessToken');
};

export const getToken = () => {
    return localStorage.getItem("accessToken");
};

// export const isAuthenticated = () => {
//     const token = getToken();

//     if (!token) return false;

//     return !isTokenExpired(token);
// };