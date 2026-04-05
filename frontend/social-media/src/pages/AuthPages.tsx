import { useState } from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useAuth } from '@/context/AuthContext'
import { TextInput, PasswordInput, PasswordStrength } from '@/components/forms/index'
import { Button } from '@/components/common/Button'
import type { LoginFormData, RegisterFormData } from '@/types'
import { PASSWORD_RULES } from '@/utils/constants'

// ─── LoginPage ────────────────────────────────────────────────────────────────
export function LoginPage() {
  const { login } = useAuth()
  const navigate  = useNavigate()
  const location  = useLocation()
  const from      = (location.state as { from?: string })?.from ?? '/'
  const [apiError, setApiError] = useState('')

  const {
    register, handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>()

  const onSubmit = async (data: LoginFormData) => {
    setApiError('')
    try {
      await login(data)
      navigate(from, { replace: true })
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message
      setApiError(msg ?? 'Invalid email or password')
    }
  }

  return (
    <AuthLayout title="Welcome back" subtitle="Sign in to your account">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
        <TextInput
          label="Email"
          type="email"
          placeholder="you@example.com"
          autoComplete="email"
          error={errors.email?.message}
          leftIcon={<MailIcon />}
          {...register('email', {
            required: 'Email is required',
            pattern:  { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'Invalid email address' },
          })}
        />

        <PasswordInput
          label="Password"
          placeholder="••••••••"
          autoComplete="current-password"
          error={errors.password?.message}
          {...register('password', { required: 'Password is required' })}
        />

        <div className="flex items-center justify-between text-sm">
          <label className="flex items-center gap-2 cursor-pointer">
            <input type="checkbox" className="rounded text-primary-600" {...register('rememberMe')} />
            <span className="text-slate-600">Remember me</span>
          </label>
          <Link to="/forgot-password" className="text-primary-600 hover:text-primary-700 font-medium">
            Forgot password?
          </Link>
        </div>

        {apiError && (
          <p className="text-sm text-red-500 bg-red-50 border border-red-200 rounded-xl px-4 py-2.5 font-medium">
            {apiError}
          </p>
        )}

        <Button type="submit" fullWidth isLoading={isSubmitting} size="lg">
          Sign in
        </Button>

        <p className="text-center text-sm text-slate-600">
          Don't have an account?{' '}
          <Link to="/register" className="text-primary-600 hover:text-primary-700 font-semibold">
            Create one
          </Link>
        </p>
      </form>
    </AuthLayout>
  )
}

// ─── RegisterPage ─────────────────────────────────────────────────────────────
export function RegisterPage() {
  const { register: registerUser } = useAuth()
  const navigate  = useNavigate()
  const [apiError, setApiError] = useState('')

  const {
    register, handleSubmit, watch,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>()

  const password = watch('password', '')

  const onSubmit = async (data: RegisterFormData) => {
    setApiError('')
    try {
      await registerUser(data)
      navigate('/', { replace: true })
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message
      setApiError(msg ?? 'Registration failed. Please try again.')
    }
  }

  return (
    <AuthLayout title="Create account" subtitle="Join the community today">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
        <TextInput
          label="Full name"
          type="text"
          placeholder="Jane Doe"
          autoComplete="name"
          error={errors.fullName?.message}
          {...register('fullName', {
            required:  'Full name is required',
            minLength: { value: 2, message: 'Must be at least 2 characters' },
          })}
        />

        <TextInput
          label="Username"
          type="text"
          placeholder="janedoe"
          autoComplete="username"
          error={errors.username?.message}
          leftIcon={<AtIcon />}
          {...register('username', {
            required:  'Username is required',
            minLength: { value: 3,  message: 'At least 3 characters' },
            maxLength: { value: 20, message: 'Max 20 characters' },
            pattern:   { value: /^[a-z0-9_]+$/, message: 'Lowercase letters, numbers, underscores only' },
          })}
        />

        <TextInput
          label="Email"
          type="email"
          placeholder="you@example.com"
          autoComplete="email"
          error={errors.email?.message}
          leftIcon={<MailIcon />}
          {...register('email', {
            required: 'Email is required',
            pattern:  { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'Invalid email' },
          })}
        />

        <div>
          <PasswordInput
            label="Password"
            placeholder="••••••••"
            autoComplete="new-password"
            error={errors.password?.message}
            {...register('password', {
              required:  'Password is required',
              minLength: { value: PASSWORD_RULES.MIN_LENGTH, message: `At least ${PASSWORD_RULES.MIN_LENGTH} characters` },
              validate: v =>
                PASSWORD_RULES.REQUIRE_UPPER.test(v) &&
                PASSWORD_RULES.REQUIRE_NUMBER.test(v) ||
                'Must include uppercase and a number',
            })}
          />
          <PasswordStrength password={password} />
        </div>

        <PasswordInput
          label="Confirm password"
          placeholder="••••••••"
          autoComplete="new-password"
          error={errors.confirmPassword?.message}
          {...register('confirmPassword', {
            required: 'Please confirm your password',
            validate: v => v === password || 'Passwords do not match',
          })}
        />

        {apiError && (
          <p className="text-sm text-red-500 bg-red-50 border border-red-200 rounded-xl px-4 py-2.5 font-medium">
            {apiError}
          </p>
        )}

        <Button type="submit" fullWidth isLoading={isSubmitting} size="lg">
          Create account
        </Button>

        <p className="text-center text-sm text-slate-600">
          Already have an account?{' '}
          <Link to="/login" className="text-primary-600 hover:text-primary-700 font-semibold">
            Sign in
          </Link>
        </p>
      </form>
    </AuthLayout>
  )
}

// ─── AuthLayout ───────────────────────────────────────────────────────────────
function AuthLayout({ title, subtitle, children }: { title: string; subtitle: string; children: React.ReactNode }) {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-primary-50/30 to-indigo-50/40 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-gradient-to-br from-primary-500 to-indigo-600 shadow-lg mb-4">
            <svg className="w-7 h-7 text-white" fill="currentColor" viewBox="0 0 24 24">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z"/>
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-slate-900">{title}</h1>
          <p className="text-slate-500 mt-1 text-sm">{subtitle}</p>
        </div>

        <div className="card p-8 shadow-md">
          {children}
        </div>
      </div>
    </div>
  )
}

// ─── Icons ─────────────────────────────────────────────────────────────────────
const MailIcon = () => (
  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
  </svg>
)

const AtIcon = () => (
  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207" />
  </svg>
)
