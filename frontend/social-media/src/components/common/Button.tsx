import type { ButtonHTMLAttributes, ReactNode } from 'react'
import { cn } from '@/utils/helpers'
import { Spinner } from './Spinner'

type Variant = 'primary' | 'secondary' | 'ghost' | 'danger' | 'outline'
type Size    = 'sm' | 'md' | 'lg'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?:   Variant
  size?:      Size
  isLoading?: boolean
  leftIcon?:  ReactNode
  rightIcon?: ReactNode
  fullWidth?: boolean
}

const variantStyles: Record<Variant, string> = {
  primary:   'bg-primary-600 hover:bg-primary-700 text-white shadow-sm',
  secondary: 'bg-slate-100 hover:bg-slate-200 text-slate-700',
  ghost:     'hover:bg-slate-100 text-slate-600',
  danger:    'bg-red-500 hover:bg-red-600 text-white shadow-sm',
  outline:   'border border-slate-200 hover:bg-slate-50 text-slate-700',
}

const sizeStyles: Record<Size, string> = {
  sm: 'text-xs px-3 py-1.5 gap-1.5',
  md: 'text-sm px-4 py-2   gap-2',
  lg: 'text-base px-5 py-2.5 gap-2.5',
}

export function Button({
  variant = 'primary',
  size    = 'md',
  isLoading = false,
  leftIcon,
  rightIcon,
  fullWidth,
  children,
  className,
  disabled,
  ...props
}: ButtonProps) {
  return (
    <button
      disabled={disabled || isLoading}
      className={cn(
        'inline-flex items-center justify-center font-semibold rounded-xl',
        'transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2',
        'disabled:opacity-50 disabled:cursor-not-allowed',
        variantStyles[variant],
        sizeStyles[size],
        fullWidth && 'w-full',
        className
      )}
      {...props}
    >
      {isLoading ? (
        <Spinner size="sm" color={variant === 'secondary' || variant === 'ghost' ? 'dark' : 'white'} />
      ) : (
        <>
          {leftIcon}
          {children}
          {rightIcon}
        </>
      )}
    </button>
  )
}
