# Frontend Setup & Running Guide

## Prerequisites
- Node.js (v16+)
- npm or yarn

## Installation

1. Install dependencies:
```bash
npm install
```

2. Create .env file based on .env.example:
```bash
cp .env.example .env
```

3. Update .env with your backend API URL:
```
REACT_APP_API_URL=http://localhost:5000/api
```

## Development

Start the development server:
```bash
npm run dev
```

The app will open at http://localhost:3000

## Building

Build for production:
```bash
npm run build
```

## Project Structure

```
src/
├── components/
│   ├── common/          # Reusable components (Input, PostSkeleton)
│   ├── layout/          # Layout components (Navbar, MainLayout)
│   └── specific/        # Feature-specific components (CreatePost, PostCard)
├── contexts/            # React Context (AuthContext)
├── hooks/               # Custom hooks (useApi, usePosts)
├── pages/               # Page components (Home, Login, Register, Profile)
├── routes/              # Routing logic (AppRoutes, ProtectedRoute)
├── services/            # API services (api.ts)
├── types/               # TypeScript interfaces
└── utils/               # Utility functions
```

## Key Features

### F1: Component Architecture
- 15+ reusable components with TypeScript
- Responsive design with Tailwind CSS
- Custom hooks for shared logic

### F2: State Management & API Integration
- React Context API for authentication state
- Axios for API requests with interceptors
- JWT token management

### F3: Forms & Validation
- React Hook Form integration
- Client-side validation
- Password strength indicators
- File upload with preview

### F4: Routing & Performance
- React Router v6 with protected routes
- Lazy loading for pages
- Loading skeletons
- Error handling

## Configuration

### Tailwind CSS
Already configured in `tailwind.config.js` and `postcss.config.js`

### TypeScript
Strict mode enabled in `tsconfig.json`

### Vite
Fast build tool configured in `vite.config.ts`

## Environment Variables

Create a `.env` file with:
```
REACT_APP_API_URL=http://localhost:5000/api
```

## API Integration

Update the backend URL in `.env` file:
- Development: `http://localhost:5000/api`
- Production: Your production API URL

## Backend API Endpoints Expected

```
POST   /api/auth/register
POST   /api/auth/login
GET    /api/posts
POST   /api/posts
PUT    /api/auth/profile
GET    /api/auth/profile
POST   /api/posts/:id/like
POST   /api/posts/:id/comment
```

## Troubleshooting

1. **Dependencies not installing**: Delete `node_modules` and run `npm install` again
2. **Port 3000 already in use**: Change in `vite.config.ts`
3. **API connection failed**: Check backend URL in `.env` file
4. **Tailwind CSS not working**: Make sure `npm install` was successful

## Next Steps

1. Connect to your backend API
2. Implement real authentication
3. Add more features (notifications, messaging, etc.)
4. Deploy to production
