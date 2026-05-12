import { useState } from 'react';
import ProductTable from '../components/Product/ProductTable';
import ProductForm from '../components/Product/ProductForm';
import {
  deleteProduct,
  getProductsByPage,
  getProductById,
  createProduct,
  updateProduct,
} from '../services/productService';
import { getAllCategories } from '../services/categoryService';

const ProductPage = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [deletingId, setDeletingId] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(12);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [pageLoaded, setPageLoaded] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);

  const loadProducts = async (page = 1, categoryId = null) => {
    setLoading(true);
    try {
      const result = await getProductsByPage(categoryId, page, pageSize);
      setProducts(result.items || []);
      setCurrentPage(result.currentPage || page);
      setTotalPages(result.totalPages || 1);
    } catch (error) {
      window.alert(error);
    } finally {
      setLoading(false);
    }
  };

  const loadCategories = async () => {
    try {
      const data = await getAllCategories();
      setCategories(data);
    } catch (error) {
      console.error(error);
    }
  };

  if (!pageLoaded) {
    loadProducts(1, null);
    loadCategories();
    setPageLoaded(true);
  }

  const handlePageChange = (newPage) => {
    setCurrentPage(newPage);
    loadProducts(newPage, selectedCategory);
  };

  const handleCategoryFilter = (categoryId) => {
    setSelectedCategory(categoryId);
    setCurrentPage(1);
    loadProducts(1, categoryId);
  };

  const handleEdit = async (product) => {
    if (!product?.id) return;
    setDetailLoading(true);
    try {
      const fullProduct = await getProductById(product.id);
      setEditingProduct(fullProduct);
      setShowForm(true);
    } catch (error) {
      window.alert('Không lấy được chi tiết sản phẩm: ' + error);
    } finally {
      setDetailLoading(false);
    }
  };

  const handleFormSubmit = async (formData) => {
    setLoading(true);
    try {
      if (editingProduct) {
        await updateProduct(formData.id, formData);
        window.alert('Cập nhật sản phẩm thành công');
      } else {
        await createProduct(formData);
        window.alert('Tạo sản phẩm thành công');
      }

      setShowForm(false);
      setEditingProduct(null);
      setCurrentPage(1);
      await loadProducts(1, selectedCategory);
    } catch (error) {
      console.error('Submit error:', error);
      window.alert(error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Bạn chắc chắn muốn xóa sản phẩm này?')) return;

    setDeletingId(id);
    try {
      await deleteProduct(id);
      window.alert('✅ Xóa sản phẩm thành công');
      await loadProducts(currentPage, selectedCategory);
    } catch (error) {
      window.alert(error);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="container-fluid py-4">
      <div className="row mb-4">
        <div className="col">
          <h1 className="mb-0">Quản lý sản phẩm</h1>
        </div>
        <div className="col-auto">
          <button
            className="btn btn-success btn-lg"
            onClick={() => {
              setEditingProduct(null);
              setShowForm(true);
            }}
            disabled={loading || showForm}
          >
            Tạo sản phẩm mới
          </button>
        </div>
      </div>

      {detailLoading && (
        <div className="alert alert-info">Đang tải chi tiết sản phẩm...</div>
      )}

      {showForm && (
        <ProductForm
          initialData={editingProduct}
          onSubmit={handleFormSubmit}
          onCancel={() => {
            setShowForm(false);
            setEditingProduct(null);
          }}
          loading={loading}
          categories={categories}
        />
      )}

      <div className="card mb-4">
        <div className="card-body">
          <label className="form-label">Lọc theo danh mục:</label>
          <select
            className="form-control"
            value={selectedCategory || ''}
            onChange={(e) =>
              handleCategoryFilter(e.target.value ? parseInt(e.target.value) : null)
            }
            disabled={loading}
          >
            <option value="">-- Tất cả danh mục --</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {loading && !showForm && (
        <div className="alert alert-info">Đang tải...</div>
      )}

      <div className="card">
        <div className="card-body p-0">
          <ProductTable
            products={products}
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
            onEdit={handleEdit}
            onDelete={handleDelete}
            deletingId={deletingId}
          />
        </div>
      </div>
    </div>
  );
};

export default ProductPage;