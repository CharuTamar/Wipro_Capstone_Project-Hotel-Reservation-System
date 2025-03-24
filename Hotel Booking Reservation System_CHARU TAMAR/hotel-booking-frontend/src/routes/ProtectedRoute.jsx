import { Navigate, Outlet } from 'react-router-dom';

const ProtectedRoute = ({ allowedRoles }) => {
  const token = document.cookie
    .split('; ')
    .find(row => row.startsWith('token='))
    ?.split('=')[1];

//   if (!token) {
//     // 🔥 Redirect to login if not authenticated
//     return <Navigate to="/login" />;
//   }

  // ✅ Decode token to extract roles (optional)
  const decodedToken = JSON.parse(atob(token.split('.')[1]));
  const userRoles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

  if (!allowedRoles || allowedRoles.some(role => userRoles?.includes(role))) {
    // ✅ Render child routes if roles are valid
    return <Outlet />;
  }

  // 🚫 Redirect if user doesn't have required roles
  return <Navigate to="/unauthorized" />;
};

export default ProtectedRoute;


// import { Navigate, Outlet, useLocation } from 'react-router-dom';

// const ProtectedRoute = ({ allowedRoles }) => {
//     const token = document.cookie
//       .split('; ')
//       .find(row => row.startsWith('token='))?.split('=')[1];

//       let userRole;

//     try {
//       if (token) {
//         const payload = JSON.parse(atob(token.split('.')[1]));
//         userRole = payload?.role;
//       }
//     } catch (error) {
//       console.error('Invalid token:', error);
//       // Redirect to login if token is invalid
//       return <Navigate to="/login" />;
//     }

//     if (!token) {
//       return <Navigate to="/login" />;
//     }

//     if (!allowedRoles.includes(userRole)) {
//       return <Navigate to="/unauthorized" />;
//     }

//     return <Outlet />;  // Renders the protected component if token and role are correct
// };

// export default ProtectedRoute;
