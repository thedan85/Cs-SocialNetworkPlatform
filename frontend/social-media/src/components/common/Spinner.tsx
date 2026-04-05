import { cn } from '@/utils/helpers'

interface SpinnerProps {
  size?:  'xs' | 'sm' | 'md' | 'lg'
  color?: 'white' | 'dark' | 'primary'
  className?: string
}

const sizeMap = { xs: 'h-3 w-3', sm: 'h-4 w-4', md: 'h-6 w-6', lg: 'h-8 w-8' }
const colorMap = {
  white:   'border-white/30 border-t-white',
  dark:    'border-slate-200 border-t-slate-600',
  primary: 'border-primary-200 border-t-primary-600',
}

export function Spinner({ size = 'md', color = 'primary', className }: SpinnerProps) {
  return (
    <div
      role="status"
      aria-label="Loading"
      className={cn(
        'animate-spin rounded-full border-2',
        sizeMap[size],
        colorMap[color],
        className
      )}
    />
  )
}
