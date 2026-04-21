# Project Guidelines

## Code Style
- Favor straightforward, readable code over clever or heavily abstracted solutions.
- Keep changes small and human-readable; prefer the simplest implementation that solves the request.
- Match the existing style in the file or feature area instead of rewriting surrounding code.
- Use clear names and explicit control flow; avoid premature optimization and unnecessary indirection.
- Add comments only when the intent is not obvious from the code itself.

## Architecture
- Backend code lives in `backend/SocialNetwork` and follows the controller -> service -> repository pattern with EF Core and MySQL.
- Frontend code lives in `frontend/src` and uses React, TypeScript, Vite, Tailwind, hooks, and services.
- Preserve existing response and API patterns, especially `ApiResponse<T>` and `ApiControllerBase` on the backend.
- Reuse established components, helpers, and extension methods before introducing new abstractions.

## Build and Test
- Backend: run `dotnet build`, `dotnet test`, and `dotnet ef database update` from `backend/SocialNetwork`.
- Frontend: run `npm run dev`, `npm run build`, and `npm run preview` from the repository root.
- Use `npm run install-all` when both the root and frontend dependencies need to be installed.

## Conventions
- Prefer the least complicated change that fully addresses the request.
- Do not add extra layers, wrappers, or generic abstractions unless they clearly improve reuse or correctness.
- Keep edits scoped to the requested behavior; avoid broad refactors or unrelated cleanup.
- When changing migrations or schema-related code, preserve the repo's MySQL/Pomelo conventions.
- See `frontend/README.md` for frontend setup details and existing project expectations.