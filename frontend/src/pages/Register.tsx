import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import Input from '../components/common/Input';
import { register as registerRequest } from '../services/auth';

interface RegisterForm {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  bio?: string;
}

const Register = () => {
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm<RegisterForm>();
  const navigate = useNavigate();

  // Watch password field to validate confirmPassword
  const password = watch('password');

  const onSubmit = async (data: RegisterForm) => {
    try {
      await registerRequest({
        userName: data.userName,
        email: data.email,
        password: data.password,
        bio: data.bio?.trim() || null
      });
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

        <Input
          label="Username"
          placeholder="john_doe"
          error={errors.userName?.message}
          {...register('userName', { required: 'Username is required' })}
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

        <div>
          <label className="text-sm font-medium text-gray-700">Bio</label>
          <textarea
            {...register('bio')}
            rows={3}
            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-200"
            placeholder="Tell us a bit about yourself"
          />
        </div>

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