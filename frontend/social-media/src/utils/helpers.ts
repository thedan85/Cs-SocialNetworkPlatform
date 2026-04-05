import { FILE_UPLOAD } from './constants'

/** Format ISO date to relative time (e.g. "2 hours ago") */
export function timeAgo(dateStr: string): string {
  const date = new Date(dateStr)
  const now  = new Date()
  const diff = (now.getTime() - date.getTime()) / 1000

  if (diff < 60)        return 'just now'
  if (diff < 3600)      return `${Math.floor(diff / 60)}m ago`
  if (diff < 86400)     return `${Math.floor(diff / 3600)}h ago`
  if (diff < 2592000)   return `${Math.floor(diff / 86400)}d ago`
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}

/** Format large numbers (e.g. 1200 → "1.2K") */
export function formatCount(n: number): string {
  if (n >= 1_000_000) return `${(n / 1_000_000).toFixed(1)}M`
  if (n >= 1_000)     return `${(n / 1_000).toFixed(1)}K`
  return String(n)
}

/** Get initials from a full name */
export function getInitials(name: string): string {
  return name
    .split(' ')
    .slice(0, 2)
    .map(n => n[0]?.toUpperCase() ?? '')
    .join('')
}

/** Validate a file for upload */
export function validateFile(file: File): string | null {
  if (!FILE_UPLOAD.ALLOWED_TYPES.includes(file.type)) {
    return `File type not allowed. Use: ${FILE_UPLOAD.ALLOWED_TYPES.join(', ')}`
  }
  const sizeMB = file.size / (1024 * 1024)
  if (sizeMB > FILE_UPLOAD.MAX_SIZE_MB) {
    return `File too large. Max ${FILE_UPLOAD.MAX_SIZE_MB}MB`
  }
  return null
}

/** Create a preview URL for a file */
export function createFilePreview(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload  = e => resolve(e.target?.result as string)
    reader.onerror = () => reject(new Error('Failed to read file'))
    reader.readAsDataURL(file)
  })
}

/** Build an avatar fallback URL using DiceBear */
export function getAvatarUrl(avatarUrl?: string, username?: string): string {
  if (avatarUrl) return avatarUrl
  return `https://api.dicebear.com/7.x/initials/svg?seed=${username ?? 'user'}`
}

/** Simple clsx-like classname merger */
export function cn(...classes: (string | boolean | undefined | null)[]): string {
  return classes.filter(Boolean).join(' ')
}

/** Truncate text to maxLen with ellipsis */
export function truncate(text: string, maxLen: number): string {
  return text.length > maxLen ? `${text.slice(0, maxLen)}…` : text
}
