# Testing

## Backend (xUnit + Moq)

- Unit and integration tests live in `backend/SocialNetwork.Tests`.
- Run all backend tests:
  - `dotnet test backend/SocialNetwork.Tests/SocialNetwork.Tests.csproj`
- Generate coverage (coverlet collector):
  - `dotnet test backend/SocialNetwork.Tests/SocialNetwork.Tests.csproj --collect:"XPlat Code Coverage"`
  - Coverage output is written under `backend/SocialNetwork.Tests/TestResults/` (look for `coverage.cobertura.xml`).

## Integration Tests

- Integration tests use `TestWebApplicationFactory` with an in-memory database.
- Authentication is provided by a test handler. Set `x-test-user-id` on requests to control the authenticated user.
- Critical workflows covered:
  - Create post
  - Notifications

## Frontend (Vitest + React Testing Library)

- Frontend tests live in `frontend/tests`.
- Run frontend tests:
  - `npm --prefix frontend test`
