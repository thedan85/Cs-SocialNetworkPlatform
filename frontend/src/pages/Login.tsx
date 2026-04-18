import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, Link } from 'react-router-dom';
import Input from '../components/common/Input';
import { useAuth } from '../contexts/AuthContext';
import { login as loginRequest, toUser } from '../services/auth';

interface LoginForm {
  userNameOrEmail: string;
  password: string;
}

const Login = () => {
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginForm>();
  const { login } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (data: LoginForm) => {
    try {
      const result = await loginRequest({
        userNameOrEmail: data.userNameOrEmail,
        password: data.password
      });
         login(result.token.accessToken, toUser(result.user), result.token.roles);
      navigate('/');
    } catch (err) {
      alert('Login failed!');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-transparent px-4">
      <form 
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-md w-full bg-white/80 backdrop-blur-xl p-8 rounded-2xl shadow-[0_24px_60px_rgba(15,23,42,0.12)] border border-white/60 space-y-6 dark:bg-slate-900/70 dark:border-slate-800/60"
      >
        <h2 className="text-3xl font-bold text-center text-slate-900 tracking-tight dark:text-slate-100">Welcome Back</h2>
        
        <Input
          label="Email or Username"
          placeholder="email@example.com"
          error={errors.userNameOrEmail?.message}
          {...register('userNameOrEmail', { required: 'Email or username is required' })}
        />

        <Input
          label="Password"
          type="password"
          placeholder="••••••••"
          error={errors.password?.message}
          {...register('password', {
            required: 'Password is required',
            minLength: { value: 6, message: 'Password must be at least 6 characters' }
          })}
        />

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 py-2 font-semibold text-white shadow-lg shadow-cyan-500/30 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none transition-all"
        >
          {isSubmitting ? 'Signing in...' : 'Sign In'}
        </button>

        <p className="text-center text-sm text-slate-600 dark:text-slate-400">
          Don't have an account? <Link to="/register" className="text-cyan-600 hover:text-cyan-700 font-medium dark:text-cyan-400 dark:hover:text-cyan-300">Sign up</Link>
        </p>
      </form>
    </div>
  );
};

export default Login;