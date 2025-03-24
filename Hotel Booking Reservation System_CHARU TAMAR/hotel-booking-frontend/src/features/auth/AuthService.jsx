import axios from 'axios';

const API_URL = 'http://localhost:5099/api/Auth';

axios.defaults.withCredentials = true;

// ✅ Get token from cookies
const getTokenFromCookie = () => {
  return document.cookie
    .split('; ')
    .find(row => row.startsWith('token='))
    ?.split('=')[1];
};

let token = getTokenFromCookie();

if (token) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}

// ✅ Register
const register = async (userData) => {
  try {
    const response = await axios.post(`${API_URL}/register`, userData, {
      withCredentials: true // ✅ Send cookies
    });
    return response.data;
  } catch (error) {
    console.error('Register Error:', error);
    throw error.response?.data || 'Registration failed';
  }
};

// ✅ Login
const login = async (userData) => {
  try {
    const response = await axios.post(`${API_URL}/login`, userData, {
      headers: {
        'Content-Type': 'application/json',
      },
      withCredentials: true // ✅ Send cookies
    });
    token = response.data.token;

    // ✅ Store token in cookie
    document.cookie = `token=${token}; path=/; samesite=lax`;

    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

    return response.data;
  } catch (error) {
    console.error('Login Error:', error);
    throw error.response?.data || 'Login failed';
  }
};

// ✅ Get Current User
const getCurrentUser = async () => {
  try {
    console.log("🔍 Current Token in Header:", axios.defaults.headers.common['Authorization']); // ✅ Log token

    if (!token) throw new Error("No token found");

    const response = await axios.get(`${API_URL}/current-user`, {
      withCredentials: true, // ✅ Send cookies
    });

    console.log("✅ Fetched user:", response.data); // ✅ Log fetched user
    return response.data;
  } catch (error) {
    console.error("❌ Failed to fetch user:", error);
    throw error;
  }
};



// ✅ Check User
const checkUser = async (email) => {
  try {
    const response = await axios.get(`${API_URL}/check-user/${email}`,{
      withCredentials: true // ✅ Send cookies
    });
    return response.data;
  } catch (error) {
    console.error('Check User Error:', error);
    throw error.response?.data || 'User check failed';
  }
};

// ✅ Assign Role
const assignRole = async (data) => {
  try {
    const response = await axios.post(`${API_URL}/assign-role`, data,{
      withCredentials: true // ✅ Send cookies
    });
    return response.data;
  } catch (error) {
    console.error('Assign Role Error:', error);
    throw error.response?.data || 'Role assignment failed';
  }
};

// ✅ Logout
const logout = () => {
  document.cookie = `token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC; samesite=lax`;
  token = null;
  delete axios.defaults.headers.common['Authorization'];
};


// ✅ Export Methods
export default { register, login, getCurrentUser, checkUser, assignRole, logout };
