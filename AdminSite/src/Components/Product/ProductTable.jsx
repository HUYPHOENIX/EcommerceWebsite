
const ProductTable = ({ 
  products = [], 
  currentPage = 1,
  totalPages = 1,
  onPageChange,
  onEdit,
  onDelete,
  deletingId
}) => {
  if (!products || products.length === 0) {
    return (
      <div className="alert alert-info">
        Không có sản phẩm nào
      </div>
    );
  }

  const getPageNumbers = () => {
    const pages = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  };

  return (
    <>
      {/* ✅ Table */}
      <table className="table table-striped table-hover mb-0">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Tên sản phẩm</th>
            <th>Giá tiền</th>
            <th>Hình Ảnh</th>
            <th>Danh mục</th>
            {(onEdit || onDelete) && <th style={{ width: '200px' }}>Hành động</th>}
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>{product.id}</td>
              <td>{product.name}</td>
              <td>
                <strong>{product.price?.toLocaleString('vi-VN')} đ</strong>
              </td>
              <td>
                {product.imageUrl ? (
                  <img 
                    src={product.imageUrl} 
                    alt={product.name}
                    style={{ width: '50px', height: '50px', objectFit: 'cover' }}
                  />
                ) : (
                  'Chưa có hình'
                )}
              </td>
              <td>{product.categoryId || '-'}</td>
              {(onEdit || onDelete) && (
                <td>
                  {onEdit && (
                    <button
                      className="btn btn-warning btn-sm me-2"
                      onClick={() => onEdit(product)}
                    >
                      Sửa
                    </button>
                  )}
                  {onDelete && (
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => onDelete(product.id)}
                      disabled={deletingId === product.id}
                    >
                      {deletingId === product.id ? 'Xóa...' : 'Xóa'}
                    </button>
                  )}
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>

      {/* ✅ Pagination */}
      {totalPages > 1 && (
        <nav aria-label="Page navigation" className="d-flex justify-content-center mt-4">
          <ul className="pagination mb-0">
            {/* Previous Button */}
            <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage === 1}
              >
                ← Trước
              </button>
            </li>

            {/* Page Numbers */}
            {getPageNumbers().map((page) => (
              <li key={page} className={`page-item ${page === currentPage ? 'active' : ''}`}>
                <button
                  className="page-link"
                  onClick={() => onPageChange(page)}
                >
                  {page}
                </button>
              </li>
            ))}

            {/* Next Button */}
            <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
              >
                Tiếp theo →
              </button>
            </li>
          </ul>
        </nav>
      )}

      {/* ✅ Info */}
      <div className="text-center mt-3 text-muted small">
        Trang {currentPage} của {totalPages}
      </div>
    </>
  );
};

export default ProductTable;