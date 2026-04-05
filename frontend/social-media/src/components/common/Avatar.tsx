import { cn, getAvatarUrl, getInitials } from '@/utils/helpers'

interface AvatarProps {
  src?:      string
  username?: string
  fullName?: string
  size?:     'xs' | 'sm' | 'md' | 'lg' | 'xl'
  className?: string
  onClick?:  () => void
}

const sizeMap = {
  xs: 'h-6 w-6 text-xs',
  sm: 'h-8 w-8 text-xs',
  md: 'h-10 w-10 text-sm',
  lg: 'h-14 w-14 text-base',
  xl: 'h-20 w-20 text-xl',
}

export function Avatar({ src, username, fullName, size = 'md', className, onClick }: AvatarProps) {
  const avatarUrl = getAvatarUrl(src, username)
  const initials  = fullName ? getInitials(fullName) : (username?.[0]?.toUpperCase() ?? '?')

  return (
    <div
      onClick={onClick}
      className={cn(
        'relative rounded-full overflow-hidden bg-gradient-to-br from-primary-500 to-indigo-600',
        'flex items-center justify-center text-white font-semibold shrink-0',
        sizeMap[size],
        onClick && 'cursor-pointer hover:opacity-90 transition-opacity',
        className
      )}
    >
      {src ? (
        <img
          src={avatarUrl}
          alt={username ?? 'User avatar'}
          className="h-full w-full object-cover"
          onError={e => {
            (e.currentTarget as HTMLImageElement).style.display = 'none'
          }}
        />
      ) : (
        <span>{initials}</span>
      )}
    </div>
  )
}
