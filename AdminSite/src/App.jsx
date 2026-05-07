import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Login from './Pages/login';
import Dashboard from './Pages/DashBoard';
import AdminLayout from './components/AdminLayout';
import ProtectedRoute from './components/ProtectedRoute';
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route 
          path="/" 
          element={
            <ProtectedRoute>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Dashboard />} />
          <Route path="categories" element={<div>Category List</div>} />
          <Route path="products" element={<div>Product List</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;