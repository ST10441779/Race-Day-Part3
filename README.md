# RaceDay Part 2 — RESTful API

A database-free ASP.NET Core 8 Web API prototype for the RaceDay event platform. It uses in-memory collections, cookie-backed server-side sessions, hashed passwords, Swagger UI and automated tests. **Data resets when the API process stops.** This version intentionally omits SQL Server and Entity Framework Core because the requested version must run without a database; the submitted rubric still requires EF Core/SQL Server for full database marks.

## Requirements
- .NET 8 SDK
- Visual Studio 2022 (ASP.NET and web development workload) or the .NET CLI

## Run locally
1. Extract the ZIP and open `RaceDay.sln` in Visual Studio.
2. Set `RaceDay.Api` as the startup project.
3. Run with **F5** or use a terminal from the solution folder:
   ```bash
   dotnet restore RaceDay.sln
   dotnet run --project src/RaceDay.Api/RaceDay.Api.csproj
   ```
4. Open the URL printed by the console and add `/swagger` (for example, `https://localhost:7xxx/swagger`). The exact port is assigned by your machine.
5. In Swagger, register an Organiser and a Participant using `/api/auth/register`, then log in using `/api/auth/login`. Swagger's browser session cookie maintains the logged-in state. Use **Try it out** for the endpoints.

## Roles
- **Organiser:** manage owned events and their categories, view enrolments for owned events, record/update results.
- **Participant:** browse events and categories, enrol in an event, view own enrolments and results.
- Both roles can view and update their own profile. Public event/category listing does not require login. Protected routes return `401` when logged out and `403` when the role is not permitted.

## Main endpoint groups
- `/api/auth` — register, login, logout, current account
- `/api/profile` — view/update own profile
- `/api/events` — list/get events; Organiser create/update/delete
- `/api/events/{eventId}/categories` — list categories; Organiser create/update/delete
- `/api/enrolments` — Participant enrolment and own enrolments; Organiser view enrolments for an owned event
- `/api/results` — Participant's results; Organiser capture/update result

All endpoint summaries and descriptions are included in Swagger. Passwords are hashed with ASP.NET Core's `PasswordHasher<User>` and are not stored as plaintext. Sessions expire after 30 minutes of inactivity.

## Run unit tests
```bash
dotnet test RaceDay.sln
```
The tests cover registration validation, failed login, unauthenticated access, role rejection, Organiser event creation and Participant enrolment.

## GitHub Actions CI/CD
The workflow at `.github/workflows/dotnet.yml` restores, builds and tests the solution on every push and pull request. After pushing the repository, open **GitHub → Actions**, wait for a successful run, and add a genuine screenshot of the green run below. A screenshot cannot be supplied until the workflow has actually run in your GitHub repository.

**CI build screenshot:** _Add your own screenshot here after the first successful GitHub Actions run._

**YouTube demonstration:** _Replace this line with your unlisted YouTube video URL after recording the demo._

## Suggested demonstration order
1. Explain the solution structure and database-free in-memory store.
2. Open Swagger and register/log in as an Organiser.
3. Create an event and add an age/distance category.
4. Log in as a Participant in a separate browser/incognito session; enrol and view own enrolments.
5. Demonstrate `401` logged-out responses and `403` wrong-role responses.
6. Capture a result as the event's Organiser and view it as the Participant.
7. Run `dotnet test RaceDay.sln` and show the test output and GitHub Actions build.

## Important submission checklist
- [ ] Push the solution to your own GitHub repository in a clearly named Part 2 branch/folder.
- [ ] Make at least 20 meaningful commits yourself; do not fabricate commit history.
- [ ] Add the real successful GitHub Actions screenshot.
- [ ] Record and link your own unlisted YouTube video.
- [ ] Confirm whether your lecturer permits the no-database variation. The assignment's database requirement is not fulfilled by this prototype.
