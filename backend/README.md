# PanoLogger Backend

Run the API from the repository root:

```powershell
dotnet run --project backend\src\PanoLogger.Api\PanoLogger.Api.csproj
```

Or from this `backend` folder:

```powershell
dotnet run --project src\PanoLogger.Api\PanoLogger.Api.csproj
```

Build the backend solution:

```powershell
dotnet build PanoLogger.Backend.slnx
```

The backend root contains a solution file, not a runnable project file, so plain `dotnet run` from this folder will not know which project to start.

Development endpoints:

- Health: `http://localhost:5195/api/health`
- Swagger UI: `http://localhost:5195/swagger`
- Register: `POST http://localhost:5195/api/auth/register`
- Login: `POST http://localhost:5195/api/auth/login`
- Development token: `POST http://localhost:5195/api/auth/dev-token`
- Logout: `POST http://localhost:5195/api/auth/logout`
- Current user: `GET http://localhost:5195/api/auth/me`

If a Debug build fails because files are locked, stop the running API process first or build Release:

```powershell
dotnet build PanoLogger.Backend.slnx -c Release
```

Supabase configuration:

Use `ConnectionStrings:DefaultConnection` for local PostgreSQL, or configure the Supabase pooled/direct PostgreSQL connection. The non-secret Supabase host details may live in appsettings, but keep `Supabase:PostgresPassword` in user secrets.

Storage uses Supabase Storage REST API with the service-role key on the backend only. Do not expose this key to the frontend.

```json
{
  "Supabase": {
    "Url": "https://your-project-ref.supabase.co",
    "ServiceRoleKey": "your-service-role-key",
    "StorageBucket": "panel-files",
    "PostgresHost": "aws-1-region.pooler.supabase.com",
    "PostgresPort": 5432,
    "PostgresDatabase": "postgres",
    "PostgresUsername": "postgres.project-ref",
    "PostgresPassword": ""
  }
}
```

```powershell
dotnet user-secrets set "Supabase:PostgresPassword" "your-database-password" --project backend\src\PanoLogger.Api\PanoLogger.Api.csproj
```

Implemented storage operations:

- Upload file to Supabase Storage
- Download file from Supabase Storage
- Delete file from Supabase Storage
- Create signed download URL
- Validate storage path, file size, content type, and extension before upload

QR management endpoints:

- `POST /api/qr/panel-codes` creates a unique panel code candidate.
- `GET /api/qr/panels/{panelId}` returns the panel public URL and QR SVG.
- `GET /api/qr/panels/{panelId}/download` downloads the QR as an SVG file.
- `GET /api/qr/panels/{panelId}/print` returns a print-friendly QR label page.
- `POST /api/qr/panels/{panelId}/regenerate` creates and saves a new unique panel code.
- `GET /api/public/panels/{panelCode}` returns public panel data for QR scans.
- `GET /api/public/panels/{panelCode}/files/{fileId}/download` redirects to a signed Supabase download URL.

The `/api/qr/*` endpoints require authentication. Login, registration, and the development token endpoint set the JWT in the `panologger_access` HttpOnly cookie.

Authorization roles and permissions:

- `SuperAdmin`: full system access.
- `CompanyAdmin`: facility, panel, file, QR, and report access.
- `FacilityManager`: panel, file, QR, and report access.
- `Viewer`: read-only report and public panel access.

Development authorization test:

```http
POST /api/auth/dev-token
Content-Type: application/json

{
  "email": "demo@panologger.com",
  "roles": ["FacilityManager"]
}
```

Use the cookie returned by Postman automatically:

```http
GET /api/auth/permissions
```

QR management requires `SuperAdmin`, `CompanyAdmin`, or `FacilityManager`. A `Viewer` session should receive `403 Forbidden` for `/api/qr/*`.

Newly registered users receive the `Viewer` role by default. Passwords are stored as hashes, authentication tokens are not returned to JavaScript or stored in local storage, and the frontend origin `http://localhost:5173` is allowed through the credentialed API CORS policy.
