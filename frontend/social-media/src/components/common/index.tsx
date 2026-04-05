import { useEffect, type ReactNode } from 'react'
import { cn } from '@/utils/helpers'

// ─── Skeleton ─────────────────────────────────────────────────────────────────
interface SkeletonProps {
  className?: string
  lines?: number
}

export function Skeleton({ className }: { className?: string }) {
  return <div className={cn('skeleton', className)} />
}

export function SkeletonText({ lines = 3 }: SkeletonProps) {
  return (
    <div className="space-y-2">
      {Array.from({ length: lines }).map((_, i) => (
        <Skeleton
          key={i}
          className={cn('h-4', i === lines - 1 ? 'w-2/3' : 'w-full')}
        />
      ))}
    </div>
  )
}

// ─── Badge ────────────────────────────────────────────────────────────────────
type BadgeVariant = 'default' | 'primary' | 'success' | 'warning' | 'danger'

interface BadgeProps {
  children: ReactNode
  variant?: BadgeVariant
  className?: string
}

const badgeVariants: Record<BadgeVariant, string> = {
  default: 'bg-slate-100 text-slate-600',
  primary: 'bg-primary-100 text-primary-700',
  success: 'bg-green-100 text-green-700',
  warning: 'bg-amber-100 text-amber-700',
  danger:  'bg-red-100  text-red-700',
}

export function Badge({ children, variant = 'default', className }: BadgeProps) {
  return (
    <span className={cn(
      'inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium',
      badgeVariants[variant],
      className
    )}>
      {children}
    </span>
  )
}

// ─── Modal ────────────────────────────────────────────────────────────────────
interface ModalProps {
  isOpen:   boolean
  onClose:  () => void
  title?:   string
  children: ReactNode
  size?:    'sm' | 'md' | 'lg'
}

const modalSizes = { sm: 'max-w-sm', md: 'max-w-md', lg: 'max-w-2xl' }

export function Modal({ isOpen, onClose, title, children, size = 'md' }: ModalProps) {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden'
    } else {
      document.body.style.overflow = ''
    }
    return () => { document.body.style.overflow = '' }
  }, [isOpen])

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/40 backdrop-blur-sm animate-fade-in"
        onClick={onClose}
      />
      {/* Panel */}
      <div className={cn(
        'relative w-full bg-white rounded-2xl shadow-xl animate-slide-up',
        modalSizes[size]
      )}>
        {title && (
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <h2 className="text-lg font-semibold text-slate-900">{title}</h2>
            <button
              onClick={onClose}
              className="p-1.5 rounded-lg hover:bg-slate-100 text-slate-500 transition-colors"
            >
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        )}
        <div className="p-6">{children}</div>
      </div>
    </div>
  )
}

// ─── Toast ────────────────────────────────────────────────────────────────────
type ToastType = 'success' | 'error' | 'info' | 'warning'

interface ToastProps {
  message:  string
  type?:    ToastType
  onClose:  () => void
}

const toastStyles: Record<ToastType, { bg: string; icon: string }> = {
  success: { bg: 'bg-green-50 border-green-200 text-green-800', icon: '✓' },
  error:   { bg: 'bg-red-50 border-red-200 text-red-800',       icon: '✕' },
  info:    { bg: 'bg-blue-50 border-blue-200 text-blue-800',    icon: 'ℹ' },
  warning: { bg: 'bg-amber-50 border-amber-200 text-amber-800', icon: '⚠' },
}

export function Toast({ message, type = 'info', onClose }: ToastProps) {
  useEffect(() => {
    const t = setTimeout(onClose, 4000)
    return () => clearTimeout(t)
  }, [onClose])

  const { bg, icon } = toastStyles[type]

  return (
    <div className={cn(
      'flex items-center gap-3 px-4 py-3 rounded-xl border shadow-lg',
      'animate-slide-up max-w-sm',
      bg
    )}>
      <span className="font-bold text-lg leading-none">{icon}</span>
      <p className="text-sm font-medium flex-1">{message}</p>
      <button onClick={onClose} className="opacity-60 hover:opacity-100 transition-opacity">
        <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  )
}

// ─── ToastContainer ───────────────────────────────────────────────────────────
export interface ToastItem {
  id:      string
  message: string
  type:    ToastType
}

export function ToastContainer({
  toasts,
  onRemove,
}: {
  toasts:   ToastItem[]
  onRemove: (id: string) => void
}) {
  return (
    <div className="fixed bottom-6 right-6 z-[100] flex flex-col gap-2">
      {toasts.map(t => (
        <Toast key={t.id} message={t.message} type={t.type} onClose={() => onRemove(t.id)} />
      ))}
    </div>
  )
}
