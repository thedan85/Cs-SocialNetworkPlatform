import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Outlet, Route, Routes, useLocation } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { Layout } from '@/components/layout/index'
import { Spinner } from '@/components/common/Spinner'

// ─── Lazy pages ───────────────────────────────────────────────────────────────
const HomePage          = lazy(() => import('@/pages/index').then(m => ({ default: m.HomePage })))
const SearchPage        = lazy(() => import('@/pages/index').then(m => ({ default: m.SearchPage })))
const ProfilePage       = lazy(() => import('@/pages/index').then(m => ({ default: m.ProfilePage })))
const NotificationsPage = lazy(() => import('@/pages/index').then(m => ({ default: m.NotificationsPage })))
const SettingsPage      = lazy(() => import('@/pages/index').then(m => ({ default: m.SettingsPage })))
const NotFoundPage      = lazy(() => import('@/pages/index').then(m => ({ default: m.NotFoundPage })))
const LoginPage         = lazy(() => import('@/pages/AuthPages').then(m => ({ default: m.LoginPage })))
const RegisterPage      = lazy(() => import('@/pages/AuthPages').then(m => ({ default: m.RegisterPage })))

// ─── Loading fallback ─────────────────────────────────────────────────────────
function PageLoader() {
  return (
    <div className="flex items-center justify-center min-h-[60vh]">
      <Spinner size="lg" />
    </div>
  )
}

// ─── ProtectedRoute ───────────────────────────────────────────────────────────
export function ProtectedRoute() {
  const { isAuthenticated, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) return <PageLoader />

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location.pathname }} replace />
  }

  return (
    <Layout>
      <Suspense fallback={<PageLoader />}>
        <Outlet />
      </Suspense>
    </Layout>
  )
}

// ─── GuestRoute ───────────────────────────────────────────────────────────────
function GuestRoute() {
  const { isAuthenticated, isLoading } = useAuth()
  if (isLoading)       return <PageLoader />
  if (isAuthenticated) return <Navigate to="/" replace />
  return (
    <Suspense fallback={<PageLoader />}>
      <Outlet />
    </Suspense>
  )
}

// ─── AppRouter ────────────────────────────────────────────────────────────────
export function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Guest-only: redirect to / if logged in */}
        <Route element={<GuestRoute />}>
          <Route path="/login"    element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Route>

        {/* Protected: wrapped in Layout */}
        <Route element={<ProtectedRoute />}>
          <Route index                     element={<HomePage />} />
          <Route path="search"             element={<SearchPage />} />
          <Route path="profile/:username"  element={<ProfilePage />} />
          <Route path="notifications"      element={<NotificationsPage />} />
          <Route path="settings"           element={<SettingsPage />} />
        </Route>

        {/* 404 */}
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  )
}
