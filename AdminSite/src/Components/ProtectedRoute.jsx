import { Navigate } from 'react-router-dom';
import { isAuthenticated } from '../services/authService';

const ProtectedRoute = ({ children }) => {
    const checkAuth = isAuthenticated();

    if (!checkAuth) {
        return <Navigate to="/login" replace />;
    }
    return children;
};

export default ProtectedRoute;