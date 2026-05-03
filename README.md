# InteractHub Social Network Platform

InteractHub is a full-stack social network application built with a React + TypeScript frontend and an ASP.NET Core Web API backend backed by MySQL. It includes authentication, posts, comments, likes, shares, profiles, friends, stories, notifications, hashtag search, admin tools, and real-time updates.

## Tech Stack

- Frontend: React 18, TypeScript, Vite, Tailwind CSS, React Router, React Hook Form, Axios, Lucide, SignalR client
- Backend: .NET 10, ASP.NET Core, EF Core, ASP.NET Identity, JWT authentication, SignalR, Swagger, MySQL with Pomelo, Dapper
## Repository Layout

- `backend/SocialNetwork` - API project, controllers, services, repositories, migrations, hubs, and configuration
- `backend/SocialNetwork.Tests` - backend unit tests
- `frontend` - Vite application for the user interface
- `frontend/src/components` - shared UI and layout components
- `frontend/src/pages` - route-level pages
- `frontend/src/services` - API clients and request helpers

## Prerequisites

- .NET 10 SDK
- Node.js LTS and npm
- MySQL 8.0 or newer
- Optional: Azure Storage account for file uploads
- Optional: `dotnet-ef` if it is not already installed globally

## Setup

1. Clone the repository.
2. Install dependencies from the repository root:

```bash
npm run install-all
```

3. Configure backend settings.

The backend reads standard appsettings values and also loads overrides from a `.env.local` file found in the backend app directory or any parent directory. A typical local setup looks like this:

```env
ConnectionStrings__DefaultConnection=Server=localhost;Database=socialnetwork;User=root;Password=
```

4. Create the database and apply migrations:

```bash
cd backend/SocialNetwork
dotnet ef database update
```

5. Start the backend API:

```bash
dotnet run
```

The API listens on `http://localhost:5245` and `https://localhost:7202` by default.

6. Start the frontend in a second terminal from the repository root:

```bash
npm run dev
```

The frontend runs on `http://localhost:3000`.

7. Open the app in your browser and sign in or register.

## Available Scripts

- `npm run dev` - start the frontend development server
- `npm run build` - build the frontend for production
- `npm run preview` - preview a production frontend build
- `npm run install-all` - install dependencies in the repository root and the frontend workspace

## Default Seed Data

On a fresh database, the backend migrates the schema and seeds the `User` and `Admin` roles plus sample content.

- Demo admin: `admin@socialnetwork.local`
- Demo password: `Demo1234!`

## Configuration Notes

- Frontend API requests default to `http://localhost:5245/api`; override this with `frontend/.env.local` using `VITE_API_URL` if needed.
- CORS is configured for `http://localhost:3000` and `http://localhost:5173`.
- The frontend stores the auth token, user, and roles in `localStorage`.
- Swagger is enabled on the backend for local API exploration.
- Uploaded images are stored on local disk in `backend/SocialNetwork/wwwroot/uploads/images` by default, and the API returns a public URL under `/uploads/images/...`.

## API Endpoints

Most endpoints require authentication. Admin-only endpoints are marked as such.

| Area | Method | Path | Notes |
| --- | --- | --- | --- |
| Auth | POST | `/api/auth/register` | Anonymous |
| Auth | POST | `/api/auth/login` | Anonymous |
| Auth | POST | `/api/auth/refresh-token` | Anonymous |
| Users | GET | `/api/users` | Admin only |
| Users | GET | `/api/users/search` | Query params: `query`, `pageNumber`, `pageSize` |
| Users | GET | `/api/users/{userId}` | Current user or admin |
| Users | PUT | `/api/users/{userId}` | Current user or admin |
| Users | GET | `/api/users/{userId}/posts` | Current user or admin |
| Uploads | POST | `/api/uploads/images` | Form upload |
| Stories | GET | `/api/stories` | Query params: `pageNumber`, `pageSize` |
| Stories | GET | `/api/stories/{storyId}` | Authenticated |
| Stories | GET | `/api/stories/user/{userId}` | Current user or admin |
| Stories | POST | `/api/stories` | Creates a story for the current user |
| Stories | DELETE | `/api/stories/{storyId}` | Current user or admin |
| Posts | GET | `/api/posts` | Query params: `pageNumber`, `pageSize` |
| Posts | GET | `/api/posts/{postId}` | Authenticated |
| Posts | POST | `/api/posts` | Creates a post for the current user |
| Posts | PUT | `/api/posts/{postId}` | Update a post |
| Posts | DELETE | `/api/posts/{postId}` | Delete a post |
| Posts | GET | `/api/posts/{postId}/comments` | Query params: `pageNumber`, `pageSize` |
| Posts | POST | `/api/posts/{postId}/comments` | Add a comment |
| Posts | DELETE | `/api/posts/{postId}/comments/{commentId}` | Delete a comment |
| Posts | POST | `/api/posts/{postId}/likes` | Like a post |
| Posts | DELETE | `/api/posts/{postId}/likes` | Unlike a post |
| Posts | POST | `/api/posts/{postId}/shares` | Share a post |
| Posts | POST | `/api/posts/{postId}/report` | Report a post |
| Friends | POST | `/api/friends/requests` | Send a friend request |
| Friends | PUT | `/api/friends/requests/{friendshipId}/accept` | Accept a request |
| Friends | PUT | `/api/friends/requests/{friendshipId}/reject` | Reject a request |
| Friends | GET | `/api/friends/relationship/{userId}` | Get relationship status |
| Friends | DELETE | `/api/friends/{friendshipId}` | Remove a friendship |
| Friends | GET | `/api/friends/{userId}` | Get accepted friends |
| Friends | GET | `/api/friends/requests/{userId}` | Get pending requests |
| Hashtags | GET | `/api/hashtags/search` | Query params: `query`, `pageNumber`, `pageSize`, `postsPerHashtag` |
| Hashtags | GET | `/api/hashtags/trending` | Query params: `pageNumber`, `pageSize` |
| Notifications | GET | `/api/notifications/user/{userId}` | Current user or admin |
| Notifications | GET | `/api/notifications/user/{userId}/unread` | Current user or admin |
| Notifications | POST | `/api/notifications` | Create a notification |
| Notifications | PUT | `/api/notifications/{notificationId}/read` | Mark as read |
| Notifications | DELETE | `/api/notifications/{notificationId}` | Delete a notification |
| Admin reports | GET | `/api/post-reports/pending` | Admin only |
| Admin reports | PUT | `/api/post-reports/{postReportId}/review` | Admin only |

## Screens

- Public: login, register
- Authenticated: home feed, notifications, friends, stories, profile, and user profile pages
- Admin: admin dashboard

## Testing

- Backend tests: `dotnet test backend/SocialNetwork.Tests/SocialNetwork.Tests.csproj`
- Frontend production build check: `npm run build`

## Troubleshooting

- If migrations fail, verify that MySQL is running and the connection string matches your local instance.
- If the frontend cannot reach the API, confirm `VITE_API_URL` and the backend CORS settings.
- If uploads are not configured, the app uses a no-op file storage implementation for local development.