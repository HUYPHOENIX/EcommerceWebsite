// components/CategoryForm.jsx
import { useState } from 'react';

const CategoryForm = ({ initialData, onSubmit, onCancel, loading }) => {
  const [formData, setFormData] = useState(initialData || {
    id: 0,
    name: '',
    description: ''
  });

  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};

    if (!formData.name?.trim()) {
      newErrors.name = 'Tên danh mục không được để trống';
    }
    if (formData.name?.length > 50) {
      newErrors.name = 'Tên danh mục không quá 50 ký tự';
    }
    if (formData.description?.length > 200) {
      newErrors.description = 'Mô tả không quá 200 ký tự';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (validateForm()) {
      onSubmit(formData);
    }
  };

  return (
    <div className="card mb-4">
      <div className="card-header bg-primary text-white">
        <h5 className="mb-0">
          {initialData?.id ? 'Cập nhật danh mục' : 'Tạo danh mục mới'}
        </h5>
      </div>
      <div className="card-body">
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label">
              Tên danh mục <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className={`form-control ${errors.name ? 'is-invalid' : ''}`}
              name="name"
              value={formData.name || ''}
              onChange={handleChange}
              placeholder="Nhập tên danh mục"
              disabled={loading}
            />
            {errors.name && (
              <div className="invalid-feedback d-block">
                {errors.name}
              </div>
            )}
          </div>

          <div className="mb-3">
            <label className="form-label">Mô tả</label>
            <textarea
              className={`form-control ${errors.description ? 'is-invalid' : ''}`}
              name="description"
              value={formData.description || ''}
              onChange={handleChange}
              placeholder="Nhập mô tả danh mục"
              rows={4}
              disabled={loading}
            />
            {errors.description && (
              <div className="invalid-feedback d-block">
                {errors.description}
              </div>
            )}
          </div>

          {/* Buttons */}
          <div className="d-flex gap-2">
            <button
              type="submit"
              className="btn btn-success"
              disabled={loading}
            >
              {loading ? 'Đang xử lý...' : (initialData?.id ? 'Cập nhật' : 'Tạo mới')}
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onCancel}
              disabled={loading}
            >
              Hủy
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CategoryForm;