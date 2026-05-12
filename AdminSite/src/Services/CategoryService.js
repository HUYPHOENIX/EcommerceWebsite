
import axios from 'axios';
import { getToken } from './AuthService';
const API_BASE_URL = 'http://localhost:5007/api/categories';


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

export const getAllCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/GetAll`);
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const getCategoryById = async (id) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/Get/${id}`);
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const createCategory = async (data) => {
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

export const updateCategory = async (id, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/${id}`,
      { ...data, id },
      getAuthConfig()
    );
    console.log(id)
    return response.data;
  } catch (error) {
    throw error.message;
  }
};

export const deleteCategory = async (id) => {
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