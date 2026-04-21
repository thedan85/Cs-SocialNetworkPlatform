import { lazy, Suspense } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import AdminRoute from './AdminRoute';
import MainLayout from '../components/layout/MainLayout';

// Lazy loading pages to optimize performance (Requirement F4)
const Home = lazy(() => import('../pages/Home'));
const Login = lazy(() => import('../pages/Login'));
const Register = lazy(() => import('../pages/Register'));
const Profile = lazy(() => import('../pages/Profile'));
const UserProfile = lazy(() => import('../pages/UserProfile'));
const Notifications = lazy(() => import('../pages/Notifications'));
const Friends = lazy(() => import('../pages/Friends'));
const Stories = lazy(() => import('../pages/Stories'));
const AdminDashboard = lazy(() => import('../pages/AdminDashboard'));

const AppRoutes = () => {
  return (
    // Suspense shows loading while pages are being loaded
    <Suspense fallback={<div className="h-screen flex items-center justify-center text-lg font-semibold">Loading page...</div>}>
      <Routes>
        {/* Public Routes: Anyone can access */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* Private Routes: Must be authenticated to access */}
        <Route element={<ProtectedRoute />}>
          <Route element={<MainLayout />}>
            <Route path="/" element={<Home />} />
            <Route path="/notifications" element={<Notifications />} />
            <Route path="/friends" element={<Friends />} />
            <Route path="/stories" element={<Stories />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/users/:userId" element={<UserProfile />} />
            <Route element={<AdminRoute />}>
              <Route path="/admin" element={<AdminDashboard />} />
            </Route>
          </Route>
        </Route>

        {/* Redirects to home if accessing invalid route */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Suspense>
  );
};

export default AppRoutes;