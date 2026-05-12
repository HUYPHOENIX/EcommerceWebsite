import { useEffect, useState } from 'react';
import CategoryTable from '../components/Category/CategoryTable';
import CategoryForm from '../components/Category/CategoryForm';
import {
  getAllCategories,
  createCategory,
  updateCategory,
  deleteCategory
} from '../services/categoryService';

const CategoryPage = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editingCategory, setEditingCategory] = useState(null);
  const [deletingId, setDeletingId] = useState(null);

  useEffect(() => {
    const loadCategories = async () => {
      setLoading(true);
      try {
        console.log('Loading categories...');
        const data = await getAllCategories();
        console.log('Data loaded:', data);
        setCategories(data);
      } catch (error) {
        console.error('Error:', error);
        window.alert('' + error);
      } finally {
        setLoading(false);
      }
    };

    loadCategories();
  }, []); 

  // ✅ Handle create/update
  const handleFormSubmit = async (formData) => {
    setLoading(true);
    try {
      if (editingCategory) {
        await updateCategory(formData.id, formData);
        window.alert('Cập nhật thành công');
      } else {
        await createCategory(formData);
        window.alert('Tạo thành công');
      }

      setShowForm(false);
      setEditingCategory(null);
      
      const data = await getAllCategories();
      setCategories(data);
    } catch (error) {
      window.alert(error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Xóa danh mục này?')) {
      return;
    }

    setDeletingId(id);
    try {
      await deleteCategory(id);
      window.alert('Xóa thành công');
      
      const data = await getAllCategories();
      setCategories(data);
    } catch (error) {
      window.alert( + error);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="container-fluid py-4">
      <div className="row mb-4">
        <div className="col">
          <h1 className="mb-0">Quản lý danh mục</h1>
        </div>
        <div className="col-auto">
          <button
            className="btn btn-success btn-lg"
            onClick={() => {
              setEditingCategory(null);
              setShowForm(true);
            }}
            disabled={loading || showForm}
          >
            Tạo mới
          </button>
        </div>
      </div>

      {showForm && (
        <CategoryForm
          initialData={editingCategory}
          onSubmit={handleFormSubmit}
          onCancel={() => {
            setShowForm(false);
            setEditingCategory(null);
          }}
          loading={loading}
        />
      )}

      {loading && !showForm && (
        <div className="alert alert-info">Đang tải...</div>
      )}

      <div className="card">
        <div className="card-body p-0">
          <CategoryTable
            categories={categories}
            onEdit={(category) => {
              setEditingCategory(category);
              setShowForm(true);
            }}
            onDelete={handleDelete}
            deletingId={deletingId}
          />
        </div>
      </div>
    </div>
  );
};

export default CategoryPage;