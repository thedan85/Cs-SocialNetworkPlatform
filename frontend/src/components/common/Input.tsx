import React, { InputHTMLAttributes, forwardRef } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

// Sử dụng forwardRef để làm việc mượt mà với React Hook Form
const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, ...props }, ref) => {
    return (
      <div className="flex flex-col gap-1 w-full">
        <label className="text-sm font-medium text-slate-600 dark:text-slate-300">{label}</label>
        <input
          ref={ref}
          className={`px-4 py-2 rounded-lg border bg-white/80 text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 transition-all dark:bg-slate-900/60 dark:text-slate-100 dark:placeholder:text-slate-500
            ${error ? 'border-rose-400 focus:ring-rose-200 dark:border-rose-500/60 dark:focus:ring-rose-500/30' : 'border-slate-200 focus:ring-cyan-300/70 dark:border-slate-700/60 dark:focus:ring-cyan-500/40'}`}
          {...props}
        />
        {error && <span className="text-xs text-rose-500 dark:text-rose-400">{error}</span>}
      </div>
    );
  }
);

Input.displayName = 'Input';
export default Input;