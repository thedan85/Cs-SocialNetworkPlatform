import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import Input from '../components/common/Input';
import { register as registerRequest } from '../services/auth';

interface RegisterForm {
  firstName: string;
  lastName: string;
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
        firstName: data.firstName,
        lastName: data.lastName,
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
    <div className="min-h-screen flex items-center justify-center bg-transparent py-12 px-4">
      <form 
        onSubmit={handleSubmit(onSubmit)}
        className="max-w-md w-full bg-white/80 backdrop-blur-xl p-8 rounded-2xl shadow-[0_24px_60px_rgba(15,23,42,0.12)] border border-white/60 space-y-4 dark:bg-slate-900/70 dark:border-slate-800/60"
      >
        <h2 className="text-3xl font-bold text-center text-slate-900 tracking-tight dark:text-slate-100">Create Account</h2>

        <Input
          label="First Name"
          placeholder="Jane"
          error={errors.firstName?.message}
          {...register('firstName', { required: 'First name is required' })}
        />

        <Input
          label="Last Name"
          placeholder="Doe"
          error={errors.lastName?.message}
          {...register('lastName', { required: 'Last name is required' })}
        />

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
            <div className="text-xs mt-1 text-slate-500 dark:text-slate-400">
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
          <label className="text-sm font-medium text-slate-600 dark:text-slate-300">Bio</label>
          <textarea
            {...register('bio')}
            rows={3}
            className="mt-1 w-full rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
            placeholder="Tell us a bit about yourself"
          />
        </div>

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 py-2 font-semibold text-white shadow-lg shadow-cyan-500/30 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none transition-all"
        >
          {isSubmitting ? 'Registering...' : 'Register'}
        </button>

        <p className="text-center text-sm text-slate-600 dark:text-slate-400">
          Already have an account? <Link to="/login" className="text-cyan-600 hover:text-cyan-700 font-medium dark:text-cyan-400 dark:hover:text-cyan-300">Sign in</Link>
        </p>
      </form>
    </div>
  );
};

export default Register;