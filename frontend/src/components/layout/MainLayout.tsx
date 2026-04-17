import React from 'react';
import Navbar from './Navbar';
import { Outlet } from 'react-router-dom';

const MainLayout: React.FC = () => {
  return (
    <div className="min-h-screen text-slate-900 dark:text-slate-100">
      <Navbar />
      <main className="max-w-6xl mx-auto px-4 py-8">
        <Outlet /> {/* This is where pages like Home, Profile will be rendered */}
      </main>
    </div>
  );
};

export default MainLayout;