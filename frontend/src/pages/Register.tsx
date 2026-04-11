import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import Input from '../components/common/Input';

interface RegisterForm {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  avatar?: FileList;
}

const Register = () => {
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm<RegisterForm>();
  const [preview, setPreview] = useState<string | null>(null);
  const navigate = useNavigate();

  // Watch password field to validate confirmPassword
  const password = watch('password');

  // Handle image preview (Requirement F3 - File upload preview)
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setPreview(URL.createObjectURL(file));
    }
  };

  const onSubmit = async (data: RegisterForm) => {
    try {
      console.log('Registering with:', data);
      // Call API: const formData = new FormData(); ... api.post('/auth/register', formData)
      await new Promise(resolve => setTimeout(resolve, 1500));
      
      alert('Registration successful!');
      navigate('/login');
    } catch (err) {
      alert('Error occurred during registration.');
    }
  };

  // Calculate password strength
  const getPasswordStrength = (pwd: string) => {
    if (!pwd) return null;
    if (pwd.length < 6) return { level: 'Weak', color: 'text-red-500' };
    if (pwd.length < 10) return { level: 'Medium', color: 'text-orange-500' };
    return { level: 'Strong', color: 'text-green-500' };
  };

  const strength = getPasswordStrength(password);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
      <form 
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-md w-full bg-white p-8 rounded-xl shadow-lg space-y-4"
      >
        <h2 className="text-3xl font-bold text-center text-gray-800">Create Account</h2>

        {/* Upload Avatar & Preview */}
        <div className="flex flex-col items-center gap-2">
          <div className="w-24 h-24 rounded-full bg-gray-200 overflow-hidden border-2 border-blue-500">
            {preview ? (
              <img src={preview} alt="Preview" className="w-full h-full object-cover" />
            ) : (
              <div className="flex items-center justify-center h-full text-gray-400 text-xs">No Avatar</div>
            )}
          </div>
          <label className="text-sm text-blue-600 cursor-pointer hover:underline font-medium">
            Choose Avatar
            <input 
              type="file" 
              className="hidden" 
              accept="image/*"
              {...register('avatar')}
              onChange={handleFileChange}
            />
          </label>
        </div>

        <Input
          label="Username"
          placeholder="john_doe"
          error={errors.username?.message}
          {...register('username', { required: 'Username is required' })}
        />

        <Input
          label="Email"
          type="email"
          placeholder="email@example.com"
          error={errors.email?.message}
          {...register('email', { 
            required: 'Email is required',
            pattern: { value: /^\S+@\S+$/i, message: 'Invalid email format' }
          })}
        />

        <div>
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
          {/* Password Strength Indicator (Requirement F3) */}
          {strength && (
            <div className="text-xs mt-1">
              Strength: <span className={strength.color}>{strength.level}</span>
            </div>
          )}
        </div>

        <Input
          label="Confirm Password"
          type="password"
          placeholder="••••••••"
          error={errors.confirmPassword?.message}
          {...register('confirmPassword', { 
            required: 'Please confirm your password',
            validate: value => value === password || 'Passwords do not match'
          })}
        />

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded-lg transition-colors disabled:bg-gray-400"
        >
          {isSubmitting ? 'Registering...' : 'Register'}
        </button>

        <p className="text-center text-sm text-gray-600">
          Already have an account? <Link to="/login" className="text-blue-600 hover:underline font-medium">Sign in</Link>
        </p>
      </form>
    </div>
  );
};

export default Register;