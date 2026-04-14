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
      login(result.token.accessToken, toUser(result.user));
      navigate('/');
    } catch (err) {
      alert('Login failed!');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <form 
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-md w-full bg-white p-8 rounded-xl shadow-lg space-y-6"
      >
        <h2 className="text-3xl font-bold text-center text-gray-800">Welcome Back</h2>
        
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
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded-lg transition-colors disabled:bg-gray-400"
        >
          {isSubmitting ? 'Signing in...' : 'Sign In'}
        </button>

        <p className="text-center text-sm text-gray-600">
          Don't have an account? <Link to="/register" className="text-blue-600 hover:underline font-medium">Sign up</Link>
        </p>
      </form>
    </div>
  );
};

export default Login;