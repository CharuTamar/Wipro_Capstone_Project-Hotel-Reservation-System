import { createContext, useContext, useState, useEffect } from 'react';
import AuthService from '../features/auth/AuthService';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(null);
  const [user, setUser] = useState(null);

  // ✅ Read token and fetch user from token on app load
  const [role, setRole] = useState(null);

useEffect(() => {
  const token = getCookie('token');
  if (token) {
    setToken(token);
    AuthService.getCurrentUser()
      .then((userData) => {
        setUser(userData);
        setRole(userData.role); // ✅ Store user role
        console.log('User role:', userData.role);
        
      })
      .catch((error) => {
        console.error('❌ Error fetching user:', error);
        handleLogout();
      });
  }
}, [token]);

const login = async (userData) => {
  try {
    const response = await AuthService.login(userData);
    setToken(response.token);
    setUser(response.user);
    setRole(response.user.role); // ✅ Store user role
    console.log('User role:', response.user.role);
    
    document.cookie = `token=${response.token}; path=/; samesite=lax`;
    console.log('Token stored in cookie:', document.cookie);
    
  } catch (error) {
    console.error('❌ Login failed:', error);
  }
};

  const handleLogout = () => {
    // ✅ Clear cookie FIRST
    document.cookie = `token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC; samesite=lax`;
  
    // ✅ THEN clear state
    setToken(null);
    setUser(null);
  
    console.log('✅ Cookie cleared:', document.cookie);
  };
  

  // ✅ Fix getCookie function
  const getCookie = (name) => {
    console.log('🔍 Cookies:', document.cookie); // ✅ Debug cookies
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
      const cookie = parts[1].split(';')[0];
      console.log('✅ Token from cookie:', cookie);
      return cookie;
    }
  };

  return (
    <AuthContext.Provider value={{ token, user, role, login, logout: handleLogout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
