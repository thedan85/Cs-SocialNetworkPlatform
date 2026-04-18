---
name: interacthub-feature-implementation
description: "Implement or extend InteractHub product features without reworking completed code paths."
argument-hint: "What InteractHub feature should be implemented or extended?"
---

# InteractHub Feature Implementation

Use this skill when implementing or extending InteractHub features in this repository.

## Core Rules

- Read `.github/copilot-instructions.md` before making changes.
- Inspect the current codebase first and verify whether the requested feature already exists.
- If a feature is already implemented, extend the existing code path instead of rebuilding it.
- Keep changes small and follow the existing backend and frontend patterns.
- Preserve the controller -> service -> repository flow on the backend.
- Preserve `ApiControllerBase`, `ApiResponse<T>`, React, TypeScript, Vite, and Tailwind conventions on the frontend.
- For notifications, use SignalR for real-time delivery and keep persisted notification records in sync for unread and history views.
- Verify which parts of the feature set already exist and avoid touching them unless the request explicitly requires it.
- Do not touch unrelated features that are already working unless the new request explicitly requires it.

## Already Implemented Check

Before changing code, verify whether the request already exists in one of these areas:

- Auth and account flows
- Posts and media
- Stories
- Likes, comments, and shares
- Friend requests and connections
- Notifications
- Profiles and settings
- Hashtags and search
- Reports and admin moderation

## Workflow

1. Identify the exact feature request and compare it to what already exists in the repo.
2. Search the relevant backend and frontend areas before adding new code.
3. Decide whether the work belongs in the backend, the frontend, or both.
4. If the backend contract is missing, add or update the controller, DTOs, service, repository, and any required migration or model changes.
5. If the backend contract already exists, wire the frontend to it and update TypeScript types and services to match.
6. For notification-producing actions, emit SignalR events and keep the persisted notification flow aligned for unread/history views.
7. Reuse existing components, helpers, and styles instead of introducing new abstractions unless they clearly improve correctness.
8. Keep admin and moderation actions behind the proper authorization checks.
9. Validate the touched area with the appropriate build or test command.

## Decision Rules

- If the feature already exists in another form, extend it rather than creating a parallel implementation.
- If the requested change affects a toggleable action, support both directions explicitly, such as like/unlike or share/unshare.
- If the request involves user-facing notifications, use SignalR for live updates and keep the database-backed notification history consistent.
- If the request involves reporting or moderation, surface the data needed for admins to review the content and decide on the action.
- If the endpoint or state shape is unclear, inspect the current backend contract first instead of guessing.
- If the change is only partially implemented, fill the missing gaps and leave the working parts intact.

## Completion Check

- The requested feature works end to end.
- Existing implemented features were not regressed or rewritten unnecessarily.
- The backend and frontend contracts still match.
- The relevant build or test commands pass for the touched area.
- The diff stays focused on the requested behavior.
