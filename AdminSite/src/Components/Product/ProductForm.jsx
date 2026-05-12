// components/ProductForm.jsx
import { useState } from 'react';

const ProductForm = ({ initialData, onSubmit, onCancel, loading, categories = [] }) => {
  const [formData, setFormData] = useState(() => 
    initialData ? {
      name: initialData.name || '',
      description: initialData.description || '',
      price: initialData.price || 0,
      imageUrl: initialData.imageUrl || '',
      sizes: initialData.sizes || [],
      colors: initialData.colors || [],
      categoryId: initialData.categoryId || 0
    } : {
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      sizes: [],
      colors: [],
      categoryId: 0
    }
  );

  const [sizeInput, setSizeInput] = useState('');
  const [colorInput, setColorInput] = useState('');
  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};

    if (!formData.name?.trim()) {
      newErrors.name = 'Tên sản phẩm không được để trống';
    }
    if (formData.name?.length > 100) {
      newErrors.name = 'Tên sản phẩm không quá 100 ký tự';
    }
    if (formData.price <= 0) {
      newErrors.price = 'Giá phải lớn hơn 0';
    }
    if (formData.categoryId <= 0) {
      newErrors.categoryId = 'Vui lòng chọn danh mục';
    }
    if (formData.description?.length > 2000) {
      newErrors.description = 'Mô tả không quá 2000 ký tự';
    }
    if (!formData.sizes || formData.sizes.length === 0) {
      newErrors.sizes = 'Vui lòng thêm ít nhất 1 size';
    }
    if (!formData.colors || formData.colors.length === 0) {
      newErrors.colors = 'Vui lòng thêm ít nhất 1 màu';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'price' || name === 'categoryId' 
        ? parseFloat(value) || 0
        : value
    }));
  };

  const handleAddSize = () => {
    if (sizeInput.trim()) {
      const capitalizedSize = sizeInput.trim().toUpperCase();
      
      if (formData.sizes.includes(capitalizedSize)) {
        window.alert('Size này đã tồn tại!');
        return;
      }

      setFormData(prev => ({
        ...prev,
        sizes: [...(prev.sizes || []), capitalizedSize]
      }));
      setSizeInput('');
    }
  };

  const handleRemoveSize = (index) => {
    setFormData(prev => ({
      ...prev,
      sizes: prev.sizes.filter((_, i) => i !== index)
    }));
  };

  const handleAddColor = () => {
    if (colorInput.trim()) {
      const capitalizedColor = colorInput.trim().toUpperCase();
      
      if (formData.colors.includes(capitalizedColor)) {
        window.alert('Màu này đã tồn tại!');
        return;
      }

      setFormData(prev => ({
        ...prev,
        colors: [...(prev.colors || []), capitalizedColor]
      }));
      setColorInput('');
    }
  };

  const handleRemoveColor = (index) => {
    setFormData(prev => ({
      ...prev,
      colors: prev.colors.filter((_, i) => i !== index)
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (validateForm()) {
      const dataToSend = {
        name: formData.name,
        description: formData.description,
        price: formData.price,
        imageUrl: formData.imageUrl,
        sizes: formData.sizes,
        colors: formData.colors,
        categoryId: formData.categoryId
      };

      if (initialData?.id) {
        dataToSend.id = initialData.id;
      }

      onSubmit(dataToSend);
    }
  };

  return (
    <div className="card mb-4">
      <div className="card-header bg-primary text-white">
        <h5 className="mb-0">
          {initialData?.id ? 'Cập nhật sản phẩm' : 'Tạo sản phẩm mới'}
        </h5>
      </div>
      <div className="card-body">
        <form onSubmit={handleSubmit}>
          <div className="row">
            {/* Name */}
            <div className="col-md-6 mb-3">
              <label className="form-label">
                Tên sản phẩm <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                name="name"
                value={formData.name || ''}
                onChange={handleChange}
                placeholder="Nhập tên sản phẩm"
                disabled={loading}
              />
              {errors.name && <div className="invalid-feedback d-block">{errors.name}</div>}
            </div>

            {/* Category */}
            <div className="col-md-6 mb-3">
              <label className="form-label">
                Danh mục <span className="text-danger">*</span>
              </label>
              <select
                className={`form-control ${errors.categoryId ? 'is-invalid' : ''}`}
                name="categoryId"
                value={formData.categoryId || 0}
                onChange={handleChange}
                disabled={loading}
              >
                <option value={0}>-- Chọn danh mục --</option>
                {categories.map(cat => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
              {errors.categoryId && <div className="invalid-feedback d-block">{errors.categoryId}</div>}
            </div>

            {/* Price */}
            <div className="col-md-6 mb-3">
              <label className="form-label">
                Giá <span className="text-danger">*</span>
              </label>
              <input
                type="number"
                className={`form-control ${errors.price ? 'is-invalid' : ''}`}
                name="price"
                value={formData.price || 0}
                onChange={handleChange}
                placeholder="Nhập giá"
                disabled={loading}
                min={1}
              />
              {errors.price && <div className="invalid-feedback d-block">{errors.price}</div>}
            </div>

            {/* Image URL */}
            <div className="col-md-6 mb-3">
              <label className="form-label">Ảnh URL</label>
              <input
                type="text"
                className="form-control"
                name="imageUrl"
                value={formData.imageUrl || ''}
                onChange={handleChange}
                placeholder="Nhập URL ảnh"
                disabled={loading}
              />
            </div>

            {/* Sizes */}
            <div className="col-md-6 mb-3">
              <label className="form-label">
                Kích cỡ <span className="text-danger">*</span>
              </label>
              <div className="input-group mb-2">
                <input
                  type="text"
                  className="form-control"
                  placeholder="VD: S, M, L, XL"
                  value={sizeInput}
                  onChange={(e) => setSizeInput(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddSize())}
                  disabled={loading}
                />
                <button
                  type="button"
                  className="btn btn-outline-primary"
                  onClick={handleAddSize}
                  disabled={loading || !sizeInput.trim()}
                >
                  Thêm
                </button>
              </div>
              {errors.sizes && <div className="text-danger small">{errors.sizes}</div>}
              <div className="d-flex flex-wrap gap-2">
                {formData.sizes?.map((size, index) => (
                  <span key={index} className="badge bg-info">
                    {size}
                    <button
                      type="button"
                      className="btn-close btn-close-white ms-1"
                      onClick={() => handleRemoveSize(index)}
                      disabled={loading}
                      style={{ cursor: 'pointer' }}
                    />
                  </span>
                ))}
              </div>
            </div>

            {/* Colors */}
            <div className="col-md-6 mb-3">
              <label className="form-label">
                Màu sắc <span className="text-danger">*</span>
              </label>
              <div className="input-group mb-2">
                <input
                  type="text"
                  className="form-control"
                  placeholder="VD: Đỏ, Xanh, Đen"
                  value={colorInput}
                  onChange={(e) => setColorInput(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddColor())}
                  disabled={loading}
                />
                <button
                  type="button"
                  className="btn btn-outline-primary"
                  onClick={handleAddColor}
                  disabled={loading || !colorInput.trim()}
                >
                  Thêm
                </button>
              </div>
              {errors.colors && <div className="text-danger small">{errors.colors}</div>}
              <div className="d-flex flex-wrap gap-2">
                {formData.colors?.map((color, index) => (
                  <span key={index} className="badge bg-success">
                    {color}
                    <button
                      type="button"
                      className="btn-close btn-close-white ms-1"
                      onClick={() => handleRemoveColor(index)}
                      disabled={loading}
                      style={{ cursor: 'pointer' }}
                    />
                  </span>
                ))}
              </div>
            </div>

            {/* Description */}
            <div className="col-md-12 mb-3">
              <label className="form-label">Mô tả</label>
              <textarea
                className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                name="description"
                value={formData.description || ''}
                onChange={handleChange}
                placeholder="Nhập mô tả sản phẩm"
                rows={4}
                disabled={loading}
              />
              {errors.description && <div className="invalid-feedback d-block">{errors.description}</div>}
            </div>
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

export default ProductForm;