import React, { createContext, useContext, useState, useEffect } from 'react';
import { User } from '../types';

interface AuthContextType {
  user: User | null;
  roles: string[];
  isAdmin: boolean;
  isAuthenticated: boolean;
  login: (token: string, userData: User, roles: string[], expiresAt?: string | null) => void;
  logout: () => void;
  updateUser: (updates: Partial<User>) => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [roles, setRoles] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Kiểm tra token trong localStorage khi vừa load app
    const token = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');
    const savedRoles = localStorage.getItem('roles');
    if (token && savedUser) {
      setUser(JSON.parse(savedUser));
    }
    if (token && savedRoles) {
      try {
        const parsed = JSON.parse(savedRoles);
        if (Array.isArray(parsed)) {
          setRoles(parsed.filter((role) => typeof role === 'string'));
        }
      } catch {
        setRoles([]);
      }
    }
    setLoading(false);
  }, []);

  const login = (token: string, userData: User, nextRoles: string[], expiresAt?: string | null) => {
    const safeRoles = Array.isArray(nextRoles)
      ? nextRoles.filter((role) => typeof role === 'string')
      : [];
    localStorage.setItem('token', token);
    if (expiresAt) {
      localStorage.setItem('token_expires_at', expiresAt);
    } else {
      localStorage.removeItem('token_expires_at');
    }
    localStorage.setItem('user', JSON.stringify(userData));
    localStorage.setItem('roles', JSON.stringify(safeRoles));
    setUser(userData);
    setRoles(safeRoles);
  };

  const updateUser = (updates: Partial<User>) => {
    setUser((current) => {
      if (!current) return current;
      const next = { ...current, ...updates };
      localStorage.setItem('user', JSON.stringify(next));
      return next;
    });
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('token_expires_at');
    localStorage.removeItem('user');
    localStorage.removeItem('roles');
    setUser(null);
    setRoles([]);
  };

  const isAdmin = roles.some((role) => role.toLowerCase() === 'admin');

  return (
    <AuthContext.Provider value={{ user, roles, isAdmin, isAuthenticated: !!user, login, logout, updateUser, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};