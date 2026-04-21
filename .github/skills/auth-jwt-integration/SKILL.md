---
name: auth-jwt-integration
description: "Use when implementing authentication, registration, JWT handling, protected requests, or frontend/backend auth integration in this repository."
---

# Auth and JWT Integration

Use this skill for any feature that spans the backend auth controller and the frontend auth session flow. The goal is to keep the backend contract, frontend axios client, and auth state in sync.

## Workflow

1. Inspect the backend auth controller and DTOs first.
2. Confirm the exact endpoint, HTTP verb, request body, and response shape before writing frontend code.
3. Reuse the existing backend auth patterns instead of adding a second auth flow.
4. Map the backend response into frontend types that match what the UI actually needs.
5. Use the shared axios client for API calls so bearer tokens are attached consistently.
6. Update auth state handling in the frontend so login, logout, and session restore stay aligned with the backend contract.
7. Preserve the existing local storage strategy unless the backend contract requires a different approach.
8. Ensure protected requests handle 401 responses in a predictable way.
9. If the response shape, token field name, or auth behavior is unclear, stop and ask instead of guessing.
10. Validate the affected backend or frontend code with the relevant build or test command.

## Decision Rules

- If the backend already exposes the needed auth endpoint, wire the frontend to it rather than creating a duplicate path.
- If the backend token or user response shape does not match the frontend types, fix the contract first and then update the client.
- If the app already has a shared axios instance, use it instead of creating a new client per feature.
- If auth state is stored in local storage, keep the stored values consistent with the backend login response.
- If the feature adds role-based or protected access, verify authorization behavior before finishing.

## Completion Check

The task is complete when the backend auth contract, frontend axios call, stored session data, and protected request behavior all agree and the affected code builds cleanly.
