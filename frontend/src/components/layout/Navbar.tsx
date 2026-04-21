import React, { useEffect, useRef, useState } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Menu, X, Home, User, Bell, LogOut, Search, Users, BookOpen, Sun, Moon, ShieldCheck } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { NavItem } from '../../types/nav';
import { searchUsers } from '../../services/users';
import { searchHashtags } from '../../services/hashtags';
import type { HashtagSearchResult, User as UserType } from '../../types';
import SearchResultsPanel from './SearchResultsPanel';
import { NotificationDropdown } from './Notification';

const Navbar: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [isNotificationsOpen, setIsNotificationsOpen] = useState(false);
  const [theme, setTheme] = useState<'light' | 'dark'>('dark');
  const [searchQuery, setSearchQuery] = useState('');
  const [searchOpen, setSearchOpen] = useState(false);
  const [searching, setSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);
  const [userResults, setUserResults] = useState<UserType[]>([]);
  const [hashtagResults, setHashtagResults] = useState<HashtagSearchResult[]>([]);
  const [history, setHistory] = useState<string[]>([]);
  const searchRef = useRef<HTMLDivElement | null>(null);
  const { logout, isAdmin } = useAuth();

  useEffect(() => {
    const stored = localStorage.getItem('theme');
    if (stored === 'light' || stored === 'dark') {
      setTheme(stored);
      return;
    }
    if (window.matchMedia?.('(prefers-color-scheme: dark)').matches) {
      setTheme('dark');
    }
  }, []);

  useEffect(() => {
    const isDark = theme === 'dark';
    document.documentElement.classList.toggle('dark', isDark);
    localStorage.setItem('theme', theme);
  }, [theme]);

  useEffect(() => {
    const stored = localStorage.getItem('searchHistory');
    if (!stored) {
      return;
    }

    try {
      const parsed = JSON.parse(stored);
      if (Array.isArray(parsed)) {
        setHistory(parsed.filter((item) => typeof item === 'string'));
      }
    } catch {
      setHistory([]);
    }
  }, []);

  useEffect(() => {
    if (!searchOpen) {
      return;
    }

    const handleClick = (event: MouseEvent) => {
      if (!searchRef.current) {
        return;
      }

      if (!searchRef.current.contains(event.target as Node)) {
        setSearchOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, [searchOpen]);

  useEffect(() => {
    if (!searchOpen) {
      return;
    }

    const trimmed = searchQuery.trim();
    if (!trimmed) {
      setUserResults([]);
      setHashtagResults([]);
      setSearchError(null);
      return;
    }

    const handler = window.setTimeout(() => {
      runSearch(trimmed, false).catch(() => undefined);
    }, 300);

    return () => window.clearTimeout(handler);
  }, [searchQuery, searchOpen]);

  const toggleTheme = () => {
    setTheme((current) => (current === 'dark' ? 'light' : 'dark'));
  };

  const updateHistory = (value: string) => {
    const trimmed = value.trim();
    if (!trimmed) {
      return;
    }

    setHistory((current) => {
      const next = [
        trimmed,
        ...current.filter((item) => item.toLowerCase() !== trimmed.toLowerCase())
      ].slice(0, 6);
      localStorage.setItem('searchHistory', JSON.stringify(next));
      return next;
    });
  };

  const clearHistory = () => {
    setHistory([]);
    localStorage.removeItem('searchHistory');
  };

  const runSearch = async (value: string, persistHistory: boolean) => {
    const trimmed = value.trim();
    if (!trimmed) {
      return;
    }

    setSearching(true);
    setSearchError(null);
    try {
      const [users, hashtags] = await Promise.all([
        searchUsers(trimmed, 1, 5),
        searchHashtags(trimmed, 1, 5, 3)
      ]);
      setUserResults(users);
      setHashtagResults(hashtags);
      if (persistHistory) {
        updateHistory(trimmed);
      }
    } catch (err) {
      setSearchError('Unable to search right now.');
    } finally {
      setSearching(false);
    }
  };

  const handleSearchSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    await runSearch(searchQuery, true);
  };

  const handleHistorySelect = async (value: string) => {
    setSearchQuery(value);
    await runSearch(value, true);
  };

  // Loại bỏ Notifications khỏi mảng này vì chúng ta sẽ xử lý nó như một Button dropdown
  const navLinks: NavItem[] = [
    { label: 'Home', href: '/', icon: Home },
    { label: 'Notifications', href: '/notifications', icon: Bell },
    { label: 'Friends', href: '/friends', icon: Users },
    { label: 'Stories', href: '/stories', icon: BookOpen },
    { label: 'Profile', href: '/profile', icon: User },
    ...(isAdmin ? [{ label: 'Admin', href: '/admin', icon: ShieldCheck }] : [])
  ];

  const activeClass = "flex items-center gap-2 text-teal-700 font-semibold bg-white/80 border border-teal-100 px-3 py-2 rounded-full shadow-sm shadow-teal-500/10 dark:text-teal-200 dark:bg-slate-900/70 dark:border-teal-500/30";
  const inactiveClass = "flex items-center gap-2 text-slate-600 hover:text-slate-900 hover:bg-white/70 px-3 py-2 rounded-full transition-all dark:text-slate-300 dark:hover:text-white dark:hover:bg-slate-800/70";

  return (
    <nav className="bg-white/70 backdrop-blur-xl border-b border-white/60 shadow-[0_8px_30px_rgba(15,23,42,0.08)] sticky top-0 z-50 dark:bg-slate-950/70 dark:border-slate-800/70 dark:shadow-[0_8px_30px_rgba(0,0,0,0.35)]">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 text-xl sm:text-2xl font-semibold tracking-tight">
            <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-gradient-to-br from-teal-500 via-cyan-500 to-amber-400 text-white text-sm font-bold shadow-lg shadow-cyan-500/30">
              IH
            </span>
            <span className="bg-gradient-to-r from-slate-900 via-slate-700 to-slate-500 bg-clip-text text-transparent dark:from-slate-100 dark:via-slate-200 dark:to-slate-400">
              InteractHub
            </span>
          </Link>

          {/* Desktop Search Bar (Hidden on Mobile) */}
          <div className="hidden md:flex flex-1 justify-center px-6">
            <div
              ref={searchRef}
              className="relative w-full min-w-[18rem] max-w-lg text-slate-400 focus-within:text-cyan-500"
            >
              <Search className="absolute left-3 top-3 w-4 h-4" />
              <form onSubmit={handleSearchSubmit} className="w-full">
                <input 
                  type="text" 
                  placeholder="Search people or hashtags..." 
                  value={searchQuery}
                  onChange={(event) => setSearchQuery(event.target.value)}
                  onFocus={() => setSearchOpen(true)}
                  className="w-full rounded-full bg-white/80 border border-white/70 py-2.5 pl-10 pr-4 text-base text-slate-700 placeholder:text-slate-400 focus:ring-2 focus:ring-cyan-300/70 focus:border-cyan-200 outline-none transition-all dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-200 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
                />
              </form>
              {searchOpen && (
                <div className="absolute left-0 right-0 mt-2 rounded-2xl border border-white/60 bg-white/90 p-4 shadow-[0_18px_40px_rgba(15,23,42,0.18)] backdrop-blur-xl dark:border-slate-800/70 dark:bg-slate-950/90">
                  <SearchResultsPanel
                    query={searchQuery}
                    searching={searching}
                    error={searchError}
                    users={userResults}
                    hashtags={hashtagResults}
                    history={history}
                    onSelectHistory={handleHistorySelect}
                    onClearHistory={clearHistory}
                  />
                </div>
              )}
            </div>
          </div>

          {/* Desktop Menu */}
          <div className="hidden md:flex items-center gap-4">
            {navLinks.map((link) => (
              <NavLink
                key={link.href}
                to={link.href}
                className={({ isActive }) =>
                  isActive ? activeClass : inactiveClass
                }
              >
                <link.icon className="w-5 h-5" />
                <span>{link.label}</span>
              </NavLink>
            ))}
            <button
              onClick={toggleTheme}
              className="ml-2 text-slate-500 hover:text-slate-900 p-2 transition-colors dark:text-slate-300 dark:hover:text-white"
              title={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
              type="button"
            >
              {theme === 'dark' ? <Sun className="w-5 h-5" /> : <Moon className="w-5 h-5" />}
            </button>
            <button 
              onClick={logout}
              className="ml-1 text-slate-500 hover:text-rose-500 p-2 transition-colors dark:text-slate-300 dark:hover:text-rose-400"
              title="Sign out"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>

          {/* Mobile Menu Button */}
          <div className="md:hidden flex items-center">
            <button onClick={() => setIsOpen(!isOpen)} className="text-slate-600 p-2 dark:text-slate-200">
              {isOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Menu Overlay */}
      {isOpen && (
        <div className="md:hidden bg-white/80 backdrop-blur-xl border-t border-white/70 py-4 px-4 space-y-2 animate-in slide-in-from-top duration-300 dark:bg-slate-950/80 dark:border-slate-800/70">
          {navLinks.map((link) => (
            <NavLink
              key={link.href}
              to={link.href}
              onClick={() => setIsOpen(false)}
              className={({ isActive }) =>
                isActive ? activeClass : inactiveClass
              }
            >
              <link.icon className="w-5 h-5" />
              <span>{link.label}</span>
            </NavLink>
          ))}
          <button
            onClick={() => { toggleTheme(); }}
            className="w-full flex items-center gap-2 text-slate-600 px-3 py-2 rounded-lg hover:bg-white/70 font-medium dark:text-slate-200 dark:hover:bg-slate-800/70"
            type="button"
          >
            {theme === 'dark' ? <Sun className="w-5 h-5" /> : <Moon className="w-5 h-5" />}
            <span>{theme === 'dark' ? 'Light mode' : 'Dark mode'}</span>
          </button>
          <button 
            onClick={() => { logout(); setIsOpen(false); }}
            className="w-full flex items-center gap-2 text-rose-500 px-3 py-2 rounded-lg hover:bg-rose-50 font-medium dark:text-rose-400 dark:hover:bg-rose-500/10"
          >
            <LogOut className="w-5 h-5" />
            <span>Sign Out</span>
          </button>
        </div>
      )}

      {/* Dropdown cho Mobile (hiển thị phủ lên khi kích hoạt từ menu mobile) */}
      <div className="md:hidden">
        <NotificationDropdown
          isOpen={isNotificationsOpen}
          onClose={() => setIsNotificationsOpen(false)}
        />
      </div>
    </nav>
  );
};

export default Navbar;
