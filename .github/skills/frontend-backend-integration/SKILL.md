---
name: frontend-backend-integration
description: "Use when integrating frontend and backend in this repository, especially when the frontend needs to call backend controllers through axios."
---

# Frontend/Backend Integration

Use this skill for features that span `backend/SocialNetwork` and `frontend/src`, where the frontend should consume the backend API directly through axios.

## Workflow

1. Inspect the backend controller that owns the feature.
2. Trace the related DTOs, service methods, repository calls, and API response shape.
3. Reuse the existing backend patterns, especially `ApiControllerBase` and `ApiResponse<T>`.
4. Confirm the route, HTTP verb, auth requirements, query parameters, and request body before changing the frontend.
5. Update or add frontend TypeScript types so they match the backend contract.
6. Create or update the frontend axios service layer to call the controller endpoint.
7. Keep UI changes small and consistent with the existing frontend structure and components.
8. Handle loading, empty, and error states in the frontend where needed.
9. If the endpoint contract is unclear, stop and ask for clarification before guessing.
10. Validate the change with the relevant build or test command for the affected side.

## Decision Rules

- If the backend already exposes the needed endpoint, wire the frontend to it instead of adding a duplicate route.
- If the backend contract is missing or inconsistent, fix the backend DTOs or controller first, then update the frontend.
- If shared types are duplicated, prefer aligning them with the backend contract rather than inventing new shapes in the frontend.
- If the feature crosses authentication boundaries, verify token handling and authorization behavior before finishing.

## Completion Check

The task is complete when the frontend axios call matches the backend controller contract, the TypeScript types are aligned, and the affected code builds cleanly.
