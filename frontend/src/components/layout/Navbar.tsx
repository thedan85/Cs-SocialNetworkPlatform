import React, { useState } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Menu, X, Home, User, Bell, LogOut, Search } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { NavItem } from '../../types/nav';

const Navbar: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const { user, logout } = useAuth();

  const navLinks: NavItem[] = [
    { label: 'Home', href: '/', icon: Home },
    { label: 'Notifications', href: '/notifications', icon: Bell },
    { label: 'Profile', href: '/profile', icon: User },
  ];

  const activeClass = "flex items-center gap-2 text-blue-600 font-bold bg-blue-50 px-3 py-2 rounded-lg";
  const inactiveClass = "flex items-center gap-2 text-gray-600 hover:text-blue-500 hover:bg-gray-50 px-3 py-2 rounded-lg transition-all";

  return (
    <nav className="bg-white shadow-sm sticky top-0 z-50">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          
          {/* Logo */}
          <Link to="/" className="text-2xl font-bold text-blue-600 flex items-center gap-2">
            <span className="bg-blue-600 text-white p-1 rounded-lg italic">S</span>
            SocialApp
          </Link>

          {/* Desktop Search Bar (Hidden on Mobile) */}
          <div className="hidden md:flex flex-1 max-w-xs mx-4">
            <div className="relative w-full text-gray-400 focus-within:text-blue-500">
              <Search className="absolute left-3 top-2.5 w-4 h-4" />
              <input 
                type="text" 
                placeholder="Search..." 
                className="w-full bg-gray-100 border-none rounded-full py-2 pl-10 pr-4 focus:ring-2 focus:ring-blue-400 outline-none transition-all"
              />
            </div>
          </div>

          {/* Desktop Menu */}
          <div className="hidden md:flex items-center gap-4">
            {navLinks.map((link) => (
              <NavLink 
                key={link.href} 
                to={link.href} 
                className={({ isActive }) => isActive ? activeClass : inactiveClass}
              >
                <link.icon className="w-5 h-5" />
                <span>{link.label}</span>
              </NavLink>
            ))}
            <button 
              onClick={logout}
              className="ml-4 text-gray-500 hover:text-red-500 p-2 transition-colors"
              title="Sign out"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>

          {/* Mobile Menu Button */}
          <div className="md:hidden flex items-center">
            <button onClick={() => setIsOpen(!isOpen)} className="text-gray-600 p-2">
              {isOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Menu Overlay */}
      {isOpen && (
        <div className="md:hidden bg-white border-t border-gray-100 py-4 px-4 space-y-2 animate-in slide-in-from-top duration-300">
          {navLinks.map((link) => (
            <NavLink 
              key={link.href} 
              to={link.href} 
              onClick={() => setIsOpen(false)}
              className={({ isActive }) => isActive ? activeClass : inactiveClass}
            >
              <link.icon className="w-5 h-5" />
              <span>{link.label}</span>
            </NavLink>
          ))}
          <button 
            onClick={() => { logout(); setIsOpen(false); }}
            className="w-full flex items-center gap-2 text-red-500 px-3 py-2 rounded-lg hover:bg-red-50 font-medium"
          >
            <LogOut className="w-5 h-5" />
            <span>Sign Out</span>
          </button>
        </div>
      )}
    </nav>
  );
};

export default Navbar;