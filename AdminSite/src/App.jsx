import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./Pages/login";
import Dashboard from "./Pages/DashBoard";
import CategoryPage from "./Pages/CategoryPage";
import AdminLayout from "./components/AdminLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import ProductPage from "./Pages/ProductPage";
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
          <Route path="/admin/categories" element={<CategoryPage />} />
          <Route path="/admin/products" element={<div><ProductPage /></div>} />
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
