import { forwardRef, useState, type InputHTMLAttributes, type ReactNode } from 'react'
import { cn } from '@/utils/helpers'
import { PASSWORD_RULES } from '@/utils/constants'

// ─── TextInput ────────────────────────────────────────────────────────────────
interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?:      string
  error?:      string
  helperText?: string
  leftIcon?:   ReactNode
  rightIcon?:  ReactNode
}

export const TextInput = forwardRef<HTMLInputElement, TextInputProps>(
  ({ label, error, helperText, leftIcon, rightIcon, className, id, ...props }, ref) => {
    const inputId = id ?? label?.toLowerCase().replace(/\s+/g, '-')
    return (
      <div className="w-full">
        {label && (
          <label htmlFor={inputId} className="block text-sm font-medium text-slate-700 mb-1.5">
            {label}
          </label>
        )}
        <div className="relative">
          {leftIcon && (
            <div className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">
              {leftIcon}
            </div>
          )}
          <input
            ref={ref}
            id={inputId}
            className={cn(
              'input-field',
              leftIcon  && 'pl-10',
              rightIcon && 'pr-10',
              error && 'border-red-400 focus:ring-red-400 bg-red-50',
              className
            )}
            aria-invalid={!!error}
            aria-describedby={error ? `${inputId}-error` : undefined}
            {...props}
          />
          {rightIcon && (
            <div className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400">
              {rightIcon}
            </div>
          )}
        </div>
        {error && (
          <p id={`${inputId}-error`} className="mt-1.5 text-xs text-red-500 font-medium">
            {error}
          </p>
        )}
        {!error && helperText && (
          <p className="mt-1.5 text-xs text-slate-500">{helperText}</p>
        )}
      </div>
    )
  }
)
TextInput.displayName = 'TextInput'

// ─── PasswordInput ────────────────────────────────────────────────────────────
type PasswordInputProps = Omit<TextInputProps, 'type' | 'rightIcon'>

export const PasswordInput = forwardRef<HTMLInputElement, PasswordInputProps>(
  (props, ref) => {
    const [show, setShow] = useState(false)
    const EyeIcon = () => (
      <button
        type="button"
        onClick={() => setShow(v => !v)}
        className="text-slate-400 hover:text-slate-600 transition-colors"
        tabIndex={-1}
      >
        {show ? (
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
              d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
          </svg>
        ) : (
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
              d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
              d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
        )}
      </button>
    )
    return (
      <TextInput ref={ref} {...props} type={show ? 'text' : 'password'} rightIcon={<EyeIcon />} />
    )
  }
)
PasswordInput.displayName = 'PasswordInput'

// ─── PasswordStrength ─────────────────────────────────────────────────────────
interface PasswordStrengthProps { password: string }

function getStrength(pw: string): { score: number; label: string; color: string } {
  if (!pw) return { score: 0, label: '', color: '' }
  let score = 0
  if (pw.length >= PASSWORD_RULES.MIN_LENGTH)       score++
  if (PASSWORD_RULES.REQUIRE_UPPER.test(pw))         score++
  if (PASSWORD_RULES.REQUIRE_LOWER.test(pw))         score++
  if (PASSWORD_RULES.REQUIRE_NUMBER.test(pw))        score++
  if (PASSWORD_RULES.REQUIRE_SPECIAL.test(pw))       score++

  const map: Record<number, { label: string; color: string }> = {
    0: { label: '',          color: '' },
    1: { label: 'Very weak', color: 'bg-red-500' },
    2: { label: 'Weak',      color: 'bg-orange-500' },
    3: { label: 'Fair',      color: 'bg-amber-500' },
    4: { label: 'Strong',    color: 'bg-lime-500' },
    5: { label: 'Very strong', color: 'bg-green-500' },
  }
  return { score, ...map[score] }
}

export function PasswordStrength({ password }: PasswordStrengthProps) {
  const { score, label, color } = getStrength(password)
  if (!password) return null
  return (
    <div className="mt-2">
      <div className="flex gap-1 mb-1">
        {Array.from({ length: 5 }).map((_, i) => (
          <div
            key={i}
            className={cn(
              'flex-1 h-1.5 rounded-full transition-all duration-300',
              i < score ? color : 'bg-slate-200'
            )}
          />
        ))}
      </div>
      {label && <p className={cn('text-xs font-medium', score < 3 ? 'text-red-500' : 'text-green-600')}>{label}</p>}
    </div>
  )
}

// ─── FileInput ────────────────────────────────────────────────────────────────
interface FileInputProps {
  label?:    string
  error?:    string
  preview?:  string | null
  accept?:   string
  onChange:  (files: FileList | null) => void
  helpText?: string
}

export function FileInput({ label, error, preview, accept, onChange, helpText }: FileInputProps) {
  return (
    <div className="w-full">
      {label && <label className="block text-sm font-medium text-slate-700 mb-1.5">{label}</label>}
      <div className={cn(
        'relative border-2 border-dashed rounded-xl p-4 text-center transition-colors cursor-pointer',
        'hover:border-primary-400 hover:bg-primary-50/50',
        error ? 'border-red-400 bg-red-50' : 'border-slate-200 bg-slate-50'
      )}>
        <input
          type="file"
          accept={accept}
          className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
          onChange={e => onChange(e.target.files)}
        />
        {preview ? (
          <img src={preview} alt="Preview" className="mx-auto max-h-40 rounded-lg object-cover" />
        ) : (
          <div className="text-slate-500">
            <svg className="w-8 h-8 mx-auto mb-2 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5}
                d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            <p className="text-sm font-medium">Click or drag to upload</p>
            {helpText && <p className="text-xs text-slate-400 mt-1">{helpText}</p>}
          </div>
        )}
      </div>
      {error && <p className="mt-1.5 text-xs text-red-500 font-medium">{error}</p>}
    </div>
  )
}
