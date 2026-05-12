
import axios from 'axios';
import { getToken } from './AuthService';
const API_BASE_URL = 'http://localhost:5007/api/products';


const getAuthConfig = () => {
  const token = getToken()
  
  if (!token) {
    throw new Error('No authentication token found');
  }

  return {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  };
};

export const getProductsByPage = async (categoryId, page, pageSize) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/Paged`, {
      params: {
        categoryId: categoryId || null,
        page: page || 1,
        pageSize: pageSize || 12
      }
    });
    return response.data;
  } catch (error) {
    throw error.message;
  }
};


export const getProductById = async (id) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/${id}`);
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const createProduct = async (data) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}`,
      data,
      getAuthConfig()
    );
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const updateProduct = async (id, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/${id}`,
      data,
      getAuthConfig()
    );
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const deleteProduct = async (id) => {
  try {
    await axios.delete(
      `${API_BASE_URL}/${id}`,
      getAuthConfig()
    );
    return true;
  } catch (error) {
    throw error.message;
  }
};
