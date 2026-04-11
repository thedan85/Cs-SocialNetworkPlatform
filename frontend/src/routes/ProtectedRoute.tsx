import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const ProtectedRoute = () => {
  const { isAuthenticated, loading } = useAuth();

  // Check if token exists while loading
  if (loading) return <div className="flex justify-center items-center h-screen"><div className="text-lg font-semibold">Loading...</div></div>;

  // If no user/token, redirect to login
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

export default ProtectedRoute;