import { useState, useCallback } from 'react'
import { AuthProvider } from '@/context/AuthContext'
import { NotificationProvider } from '@/context/NotificationContext'
import { AppRouter } from '@/router/index'
import { ToastContainer, type ToastItem } from '@/components/common/index'

// ─── ToastContext (lightweight, no separate file needed) ──────────────────────
import { createContext, useContext } from 'react'

interface ToastContextValue {
  toast: (message: string, type?: ToastItem['type']) => void
}

export const ToastContext = createContext<ToastContextValue>({ toast: () => {} })
export const useToast = () => useContext(ToastContext)

// ─── App ──────────────────────────────────────────────────────────────────────
export default function App() {
  const [toasts, setToasts] = useState<ToastItem[]>([])

  const toast = useCallback((message: string, type: ToastItem['type'] = 'info') => {
    const id = crypto.randomUUID()
    setToasts(prev => [...prev, { id, message, type }])
  }, [])

  const removeToast = useCallback((id: string) => {
    setToasts(prev => prev.filter(t => t.id !== id))
  }, [])

  return (
    <ToastContext.Provider value={{ toast }}>
      <AuthProvider>
        <NotificationProvider>
          <AppRouter />
          <ToastContainer toasts={toasts} onRemove={removeToast} />
        </NotificationProvider>
      </AuthProvider>
    </ToastContext.Provider>
  )
}
