// import { useEffect } from "react";
// import { useNavigate } from "react-router-dom";
// import {
//     getToken,
//     logout
// } from "../services/authService";

// import {
//     getTokenExpirationTime
// } from "../utils/tokenUtils";

// const useAuthCheck = () => {

//     const navigate = useNavigate();

//     useEffect(() => {

//         const token = getToken();

//         if (!token) {
//             navigate("/login");
//             return;
//         }

//         const remainingTime =
//             getTokenExpirationTime(token);

//         if (remainingTime <= 0) {
//             logout();
//             navigate("/login");
//             return;
//         }

//         const timer = setTimeout(() => {
//             logout();
//             navigate("/login");
//         }, remainingTime * 1000);

//         return () => clearTimeout(timer);

//     }, [navigate]);
// };

// export default useAuthCheck;