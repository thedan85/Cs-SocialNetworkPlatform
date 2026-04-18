---
name: admin-ui-creation
description: "Use when creating or extending admin UI, admin dashboard pages, role-based routing, moderation panels, or admin-only screens in this repository."
argument-hint: "What admin UI should be created or updated?"
---

# Admin UI Creation

Use this skill when the app needs a first admin surface or when existing frontend screens need admin-only controls. Follow `.github/copilot-instructions.md` first, then keep the work aligned with the existing React, TypeScript, Tailwind, and backend controller patterns in this repository.

## When to Use

- The project does not yet have an admin dashboard or admin-only route.
- An existing feature needs admin-only actions, moderation tools, or a management view.
- The frontend needs to recognize admin roles from the auth response before rendering privileged UI.

## Workflow

1. Read `.github/copilot-instructions.md` and inspect the current frontend shell in `frontend/src/routes/AppRoutes.tsx`, `frontend/src/components/layout/MainLayout.tsx`, `frontend/src/components/layout/Navbar.tsx`, and `frontend/src/contexts/AuthContext.tsx`.
2. Check the backend auth and admin contract in `backend/SocialNetwork/Controller/`, `backend/SocialNetwork/Dtos/`, and the seeded roles in `backend/SocialNetwork/Data/IdentitySeeder.cs`.
3. Confirm whether the frontend already keeps role data from login. If it does not, update the auth types and context first so the UI can reliably detect admin access.
4. Reuse the existing app styling and structure instead of building a separate design system. Keep the admin UI visually consistent with the current Tailwind patterns and layout.
5. Add or extend admin routes behind role checks. Hide admin navigation and controls from non-admin users, but keep the authenticated flow unchanged.
6. Build the smallest useful admin surface first. If no admin UI exists yet, create a simple dashboard shell and add only the sections that are backed by real backend capabilities.
7. If the admin screen needs API calls, use the shared axios client in `frontend/src/services/api.ts`, add matching service helpers, and keep the TypeScript types aligned with the backend response shapes.
8. Handle loading, empty, error, and unauthorized states explicitly.
9. If the backend endpoint, role rule, or response shape is unclear, stop and resolve the contract before inventing UI behavior.
10. Validate the change with the relevant build or test command, usually `npm run build` for frontend-only work and `dotnet build` when backend code changes.

## Decision Rules

- If an admin page already exists, extend it instead of creating a duplicate route or layout.
- If the frontend cannot identify admins yet, fix auth state first; do not hardcode role checks in one screen only.
- If the change needs backend data or mutation endpoints, follow the `frontend-backend-integration` workflow as part of the task.
- If the feature depends on token or role handling, follow the `auth-jwt-integration` workflow as part of the task.
- If only a visual shell is needed, keep the change isolated to routing, layout, and reusable components.

## Completion Check

The task is complete when admin users can reach the intended admin surface, non-admin users cannot access it, the UI matches the existing app style, and the affected code builds cleanly.