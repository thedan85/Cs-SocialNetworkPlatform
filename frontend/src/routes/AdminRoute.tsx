import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const AdminRoute = () => {
  const { isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="text-lg font-semibold">Loading...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin) {
    return (
      <div className="flex min-h-[70vh] items-center justify-center">
        <div className="max-w-md rounded-2xl border border-amber-200/70 bg-amber-50/70 px-6 py-5 text-center text-sm text-amber-700 shadow-[0_16px_40px_rgba(15,23,42,0.12)] dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200">
          <h2 className="text-lg font-semibold">Access denied</h2>
          <p className="mt-2">You don't have permission to view this page.</p>
        </div>
      </div>
    );
  }

  return <Outlet />;
};

export default AdminRoute;
