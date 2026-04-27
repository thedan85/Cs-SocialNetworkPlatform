# InteractHub Social Network Platform

InteractHub is a full-stack social network application built with a React + TypeScript frontend and an ASP.NET Core Web API backend backed by MySQL. It includes authentication, posts, comments, likes, shares, profiles, friends, stories, notifications, hashtag search, admin tools, and real-time updates.

## Tech Stack

- Frontend: React 18, TypeScript, Vite, Tailwind CSS, React Router, React Hook Form, Axios, Lucide, SignalR client
- Backend: .NET 10, ASP.NET Core, EF Core, ASP.NET Identity, JWT authentication, SignalR, Swagger, MySQL with Pomelo, Dapper
- Storage: Azure Blob Storage when configured, with a no-op local development fallback

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
ConnectionStrings__DefaultConnection=Server=localhost;Database=socialnetwork;User=root;
Jwt__SecretKey=replace-with-a-long-secret-key
Jwt__Issuer=InteractHub
Jwt__Audience=InteractHubClient
Jwt__AccessTokenMinutes=60
```

If you want real blob storage for uploads, also add:

```env
AzureBlobStorage__ConnectionString=...
AzureBlobStorage__ContainerName=interacthub-uploads
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