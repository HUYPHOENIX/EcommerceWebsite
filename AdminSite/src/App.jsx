import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./Pages/login";
import Dashboard from "./Pages/DashBoard";
import AdminLayout from "./components/AdminLayout";
import ProtectedRoute from "./components/ProtectedRoute";
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route
          path="/admin"
          element={
            <ProtectedRoute>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Dashboard />} />
          <Route path="/admin/categories" element={<div>Category List</div>} />
          <Route path="/admin/products" element={<div>Product List</div>} />
        </Route>
        <Route
          path="*"
          element={
            <div style={{ padding: 50, textAlign: "center" }}>
              <h2>404 - Trang không tồn tại</h2>
              <p>Đường dẫn này không đúng.</p>
            </div>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
