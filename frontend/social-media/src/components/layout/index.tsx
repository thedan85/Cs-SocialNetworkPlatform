import { useState, useEffect, type ReactNode } from 'react'
import { Link, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { useNotifications } from '@/context/NotificationContext'
import { useDebounce, useClickOutside } from '@/hooks/useUtils'
import { Avatar } from '@/components/common/Avatar'
import { NotificationBell } from '@/components/notifications/index'

// ─── Nav item definitions ─────────────────────────────────────────────────────
const NAV_ITEMS = [
  {
    to: '/', label: 'Home', end: true,
    icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6',
  },
  {
    to: '/search', label: 'Explore', end: false,
    icon: 'M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z',
  },
  {
    to: '/notifications', label: 'Notifications', end: false, badge: true,
    icon: 'M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9',
  },
  {
    to: '/settings', label: 'Settings', end: false,
    icon: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z M15 12a3 3 0 11-6 0 3 3 0 016 0z',
  },
]

// ─── Navbar ───────────────────────────────────────────────────────────────────
export function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [query, setQuery] = useState('')
  const [showUser, setShowUser] = useState(false)
  const debouncedQuery = useDebounce(query, 400)
  const menuRef = useClickOutside<HTMLDivElement>(() => setShowUser(false))

  useEffect(() => {
    if (debouncedQuery.trim()) {
      navigate(`/search?q=${encodeURIComponent(debouncedQuery)}`)
    }
  }, [debouncedQuery, navigate])

  return (
    <header className="sticky top-0 z-20 bg-white/80 backdrop-blur-md border-b border-slate-100 h-16">
      <div className="max-w-6xl mx-auto px-4 h-full flex items-center gap-4">
        <Link to="/" className="flex items-center gap-2 shrink-0">
          <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-primary-500 to-indigo-600 flex items-center justify-center shadow-sm">
            <svg className="w-4 h-4 text-white" fill="currentColor" viewBox="0 0 24 24">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z"/>
            </svg>
          </div>
          <span className="font-bold text-slate-900 hidden sm:block tracking-tight">SocialHub</span>
        </Link>

        <div className="flex-1 max-w-md">
          <div className="relative">
            <svg className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input
              type="text"
              value={query}
              onChange={e => setQuery(e.target.value)}
              placeholder="Search people, posts…"
              className="w-full pl-9 pr-4 py-2 rounded-xl bg-slate-100 border-0 text-sm
                focus:outline-none focus:ring-2 focus:ring-primary-500 focus:bg-white
                placeholder-slate-400 transition-all duration-200"
            />
          </div>
        </div>

        <div className="flex items-center gap-1 ml-auto">
          <NotificationBell />

          <div ref={menuRef} className="relative">
            <button
              onClick={() => setShowUser(v => !v)}
              className="flex items-center gap-2 p-1.5 rounded-xl hover:bg-slate-100 transition-colors"
            >
              <Avatar src={user?.avatarUrl} username={user?.username} fullName={user?.fullName} size="sm" />
              <svg className="w-3.5 h-3.5 text-slate-400 hidden sm:block" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            {showUser && (
              <div className="absolute right-0 mt-2 w-52 bg-white rounded-2xl shadow-xl border border-slate-100 py-1 z-30 animate-slide-up">
                <div className="px-4 py-2.5 border-b border-slate-100 mb-1">
                  <p className="text-sm font-semibold text-slate-900 truncate">{user?.fullName}</p>
                  <p className="text-xs text-slate-500 truncate">@{user?.username}</p>
                </div>

                <DropdownLink to={`/profile/${user?.username}`} onClick={() => setShowUser(false)}
                  icon="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z">
                  View profile
                </DropdownLink>
                <DropdownLink to="/settings" onClick={() => setShowUser(false)}
                  icon="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z M15 12a3 3 0 11-6 0 3 3 0 016 0z">
                  Settings
                </DropdownLink>

                <div className="border-t border-slate-100 mt-1 pt-1">
                  <button
                    onClick={() => { logout(); setShowUser(false) }}
                    className="flex items-center gap-2.5 w-full px-4 py-2 text-sm text-red-500 hover:bg-red-50 transition-colors"
                  >
                    <svg className="w-4 h-4 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    Sign out
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  )
}

function DropdownLink({
  to, icon, onClick, children,
}: { to: string; icon: string; onClick: () => void; children: ReactNode }) {
  return (
    <Link to={to} onClick={onClick}
      className="flex items-center gap-2.5 px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 transition-colors">
      <svg className="w-4 h-4 text-slate-400 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
        <path strokeLinecap="round" strokeLinejoin="round" d={icon} />
      </svg>
      {children}
    </Link>
  )
}

// ─── Sidebar ──────────────────────────────────────────────────────────────────
export function Sidebar() {
  const { user }        = useAuth()
  const { unreadCount } = useNotifications()

  return (
    <aside className="hidden lg:flex flex-col w-56 shrink-0 pt-2 gap-0.5">
      {NAV_ITEMS.map(item => (
        <NavLink
          key={item.to}
          to={item.to}
          end={item.end}
          className={({ isActive }) =>
            `flex items-center gap-3 px-3 py-2.5 rounded-xl font-medium text-sm transition-all duration-150
            ${isActive ? 'bg-primary-50 text-primary-700' : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'}`
          }
        >
          <svg className="w-5 h-5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d={item.icon} />
          </svg>
          <span className="flex-1">{item.label}</span>
          {item.badge && unreadCount > 0 && (
            <span className="bg-red-500 text-white text-[10px] font-bold rounded-full min-w-[18px] h-[18px] px-1 flex items-center justify-center leading-none">
              {unreadCount > 99 ? '99+' : unreadCount}
            </span>
          )}
        </NavLink>
      ))}

      {user && (
        <NavLink
          to={`/profile/${user.username}`}
          className={({ isActive }) =>
            `flex items-center gap-3 px-3 py-2.5 rounded-xl font-medium text-sm transition-all duration-150 mt-1
            ${isActive ? 'bg-primary-50 text-primary-700' : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'}`
          }
        >
          <Avatar src={user.avatarUrl} username={user.username} fullName={user.fullName} size="xs" />
          Profile
        </NavLink>
      )}
    </aside>
  )
}

// ─── MobileNav ────────────────────────────────────────────────────────────────
export function MobileNav() {
  const { unreadCount } = useNotifications()

  return (
    <nav className="lg:hidden fixed bottom-0 left-0 right-0 z-20 bg-white/90 backdrop-blur-md border-t border-slate-100 px-2 py-1">
      <div className="flex justify-around max-w-sm mx-auto">
        {NAV_ITEMS.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.end}
            className={({ isActive }) =>
              `relative flex flex-col items-center gap-0.5 px-3 py-2 rounded-xl text-[10px] font-medium transition-colors
              ${isActive ? 'text-primary-600' : 'text-slate-500'}`
            }
          >
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d={item.icon} />
            </svg>
            {item.label}
            {item.badge && unreadCount > 0 && (
              <span className="absolute top-1.5 right-2.5 w-2 h-2 rounded-full bg-red-500" />
            )}
          </NavLink>
        ))}
      </div>
    </nav>
  )
}

// ─── Layout ───────────────────────────────────────────────────────────────────
export function Layout({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen bg-slate-50">
      <Navbar />
      <div className="max-w-6xl mx-auto px-4 py-6 flex gap-6">
        <Sidebar />
        <main className="flex-1 min-w-0 pb-24 lg:pb-0">
          {children}
        </main>
      </div>
      <MobileNav />
    </div>
  )
}
