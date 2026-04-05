# Component Hierarchy & Architecture

## App Tree
```
App  (ToastContext.Provider)
├── AuthProvider                      src/context/AuthContext.tsx
│   └── NotificationProvider          src/context/NotificationContext.tsx
│       └── AppRouter                 src/router/index.tsx
│           ├── GuestRoute            (redirects authenticated users → /)
│           │   ├── /login     → LoginPage
│           │   └── /register  → RegisterPage
│           │
│           └── ProtectedRoute        (redirects unauthenticated → /login)
│               └── Layout
│                   ├── Navbar
│                   │   ├── Search input (debounced → /search)
│                   │   ├── NotificationBell
│                   │   │   └── NotificationItem[]
│                   │   └── User dropdown menu
│                   ├── Sidebar  (desktop, w-56)
│                   │   └── NavLink[] with unread notification badge
│                   ├── <Outlet>  (lazy-loaded pages)
│                   │   ├── /                  → HomePage
│                   │   │   ├── PostForm
│                   │   │   └── PostList → PostCard[]
│                   │   ├── /search            → SearchPage
│                   │   │   ├── UserCard[] → FriendButton
│                   │   │   └── PostList
│                   │   ├── /profile/:username → ProfilePage
│                   │   │   ├── Avatar, FriendButton
│                   │   │   ├── PostList
│                   │   │   └── Modal → EditProfileForm
│                   │   ├── /notifications     → NotificationsPage
│                   │   │   └── NotificationItem[]
│                   │   └── /settings          → SettingsPage
│                   └── MobileNav  (fixed bottom, mobile only)
└── ToastContainer  (fixed bottom-right)
```

## File Map

```
src/
├── main.tsx
├── App.tsx                           ToastContext + all providers
├── index.css                         Tailwind + .card .btn-primary .input-field
├── types/index.ts                    All TS interfaces
├── utils/constants.ts                ROUTES, TOKEN_KEY, FILE_UPLOAD rules
├── utils/helpers.ts                  timeAgo, formatCount, cn, validateFile...
├── services/
│   ├── api.ts                        Axios + auto token refresh (401 queue)
│   ├── authService.ts
│   ├── postService.ts
│   ├── userService.ts
│   ├── notificationService.ts
│   └── searchService.ts
├── context/
│   ├── AuthContext.tsx               AuthProvider + useAuth
│   └── NotificationContext.tsx       SignalR + useNotifications
├── hooks/
│   ├── index.ts                      Barrel exports
│   ├── usePosts.ts                   Paginate + optimistic like/save/delete
│   ├── useToast.ts
│   └── useUtils.ts                   useDebounce, useInfiniteScroll,
│                                     useLocalStorage, useClickOutside
├── components/
│   ├── common/
│   │   ├── Avatar.tsx
│   │   ├── Button.tsx
│   │   ├── Spinner.tsx
│   │   └── index.tsx                 Skeleton, Badge, Modal, Toast, ToastContainer
│   ├── forms/index.tsx               TextInput, PasswordInput,
│   │                                 PasswordStrength, FileInput
│   ├── posts/index.tsx               PostCard, PostSkeleton, PostForm, PostList
│   ├── users/index.tsx               UserCard, UserCardSkeleton, FriendButton
│   ├── notifications/index.tsx       NotificationBell, NotificationItem
│   └── layout/index.tsx              Navbar, Sidebar, MobileNav, Layout
├── pages/
│   ├── AuthPages.tsx                 LoginPage, RegisterPage
│   └── index.tsx                     HomePage, SearchPage, ProfilePage,
│                                     NotificationsPage, SettingsPage, NotFoundPage
└── router/index.tsx                  AppRouter, ProtectedRoute, GuestRoute
```

## Component Count: 28 components, 7 custom hooks

## Key Architecture Decisions

| Concern        | Solution                                                      |
|----------------|---------------------------------------------------------------|
| Auth state     | React Context + JWT localStorage + auto-refresh interceptor   |
| Global state   | AuthContext + NotificationContext (no Redux needed)           |
| Real-time      | SignalR HubConnection in NotificationProvider                 |
| Routing        | React Router v6 Outlet pattern + lazy imports                 |
| Forms          | React Hook Form — no controlled component boilerplate         |
| Optimistic UI  | usePosts toggleLike/toggleSave revert on API error            |
| Infinite scroll| IntersectionObserver sentinel div in PostList                 |
| Debounced search| useDebounce(400ms) in SearchPage + Navbar                    |
