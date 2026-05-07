
// import jwtDecode from 'jwt-decode';

// export const decodeToken = (token) => {
//     try {
//         return jwtDecode(token);
//     } catch (error) {
//         console.error('Invalid token:', error);
//         return null;
//     }
// };


// export const isTokenExpired = (token) => {
//     if (!token) return true;

//     const decoded = decodeToken(token);
//     if (!decoded || !decoded.exp) return true;

//     const expirationTime = decoded.exp * 1000;
//     const currentTime = Date.now();

//     return currentTime > expirationTime;
// };


// export const getTokenExpirationTime = (token) => {
//     const decoded = decodeToken(token);
//     if (!decoded || !decoded.exp) return 0;

//     const expirationTime = decoded.exp * 1000;
//     const currentTime = Date.now();
//     return Math.floor((expirationTime - currentTime) / 1000);
// };