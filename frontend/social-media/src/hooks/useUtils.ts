// ─── useDebounce ──────────────────────────────────────────────────────────────
import { useEffect, useRef, useState } from 'react'

export function useDebounce<T>(value: T, delay = 400): T {
  const [debounced, setDebounced] = useState(value)
  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(timer)
  }, [value, delay])
  return debounced
}

// ─── useInfiniteScroll ────────────────────────────────────────────────────────
export function useInfiniteScroll(
  onLoadMore: () => void,
  hasMore: boolean,
  isLoading: boolean
) {
  const observerRef = useRef<IntersectionObserver | null>(null)
  const sentinelRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    if (!hasMore || isLoading) return
    observerRef.current = new IntersectionObserver(
      ([entry]) => { if (entry.isIntersecting) onLoadMore() },
      { threshold: 0.1 }
    )
    if (sentinelRef.current) observerRef.current.observe(sentinelRef.current)
    return () => observerRef.current?.disconnect()
  }, [onLoadMore, hasMore, isLoading])

  return sentinelRef
}

// ─── useLocalStorage ──────────────────────────────────────────────────────────
export function useLocalStorage<T>(key: string, initialValue: T) {
  const [value, setValue] = useState<T>(() => {
    try {
      const item = localStorage.getItem(key)
      return item ? (JSON.parse(item) as T) : initialValue
    } catch {
      return initialValue
    }
  })

  const set = (v: T | ((prev: T) => T)) => {
    const next = v instanceof Function ? v(value) : v
    setValue(next)
    localStorage.setItem(key, JSON.stringify(next))
  }

  return [value, set] as const
}

// ─── useClickOutside ─────────────────────────────────────────────────────────
export function useClickOutside<T extends HTMLElement>(
  callback: () => void
) {
  const ref = useRef<T | null>(null)
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) callback()
    }
    document.addEventListener('mousedown', handler)
    return () => document.removeEventListener('mousedown', handler)
  }, [callback])
  return ref
}
