// components/CategoryTable.jsx
const CategoryTable = ({ categories, onEdit, onDelete, deletingId }) => {
  if (!categories || categories.length === 0) {
    return (
      <div className="alert alert-info">
        Không có danh mục nào
      </div>
    );
  }

  return (
    <table className="table table-striped table-hover">
      <thead className="table-dark">
        <tr>
          <th>ID</th>
          <th>Tên danh mục</th>
          <th>Mô tả</th>
          <th style={{ width: '200px' }}>Hành động</th>
        </tr>
      </thead>
      <tbody>
        {categories.map((category) => (
          <tr key={category.id}>
            <td>{category.id}</td>
            <td>{category.name}</td>
            <td>{category.description || '-'}</td>
            <td>
              <button
                className="btn btn-warning btn-sm me-2"
                onClick={() => onEdit(category)}
              >
                Sửa
              </button>
              <button
                className="btn btn-danger btn-sm"
                onClick={() => onDelete(category.id)}
                disabled={deletingId === category.id}
              >
                {deletingId === category.id ? 'Xóa...' : ' Xóa'}
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default CategoryTable;