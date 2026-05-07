
import { Link, Outlet , useNavigate } from 'react-router-dom';
import {logout} from '../services/authService';
const AdminLayout = () => {
    const navigate = useNavigate();
    const handleLogout = () => {
        logout();
        navigate('/login');
    }
    return (
        <div>
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
                <div className="container-fluid">
                    <Link className="navbar-brand" to="/">Admin</Link>
                    <div className="collapse navbar-collapse">
                        <ul className="navbar-nav me-auto">
                            <li className="nav-item">
                                <Link className="nav-link" to="/categories">Categories</Link>
                            </li>
                            <li className="nav-item">
                                <Link className="nav-link" to="/products">Products</Link>
                            </li>
                        </ul>
                        <button className="btn btn-outline-light btn-sm" onClick={handleLogout}>
                            Logout
                        </button>
                    </div>
                </div>
            </nav>
            <div className="container">
                <Outlet />
            </div>
        </div>
    );
};

export default AdminLayout;