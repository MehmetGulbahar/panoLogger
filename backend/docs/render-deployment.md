# Render Docker Deployment

Use this when deploying the backend API to Render.

## Service

- Service type: Web Service
- Runtime: Docker
- Root directory: `backend`
- Dockerfile path: `Dockerfile`
- Health check path: `/api/health`

The API reads Render's `PORT` environment variable in `Program.cs`. If `PORT` is not set, the container falls back to port `8080`.

## Required Environment Variables

```text
ASPNETCORE_ENVIRONMENT=Production

Supabase__Url=https://your-project-ref.supabase.co
Supabase__ServiceRoleKey=your-service-role-key
Supabase__StorageBucket=panel-files

Supabase__PostgresHost=your-supabase-pooler-host
Supabase__PostgresPort=5432
Supabase__PostgresDatabase=postgres
Supabase__PostgresUsername=postgres.your-project-ref
Supabase__PostgresPassword=your-real-supabase-db-password

Jwt__SigningKey=replace-with-a-long-random-production-secret
Jwt__Issuer=PanoLogger
Jwt__Audience=PanoLogger.Client

Cors__AllowedOrigins__0=https://your-vercel-app.vercel.app
Qr__PublicAppBaseUrl=https://your-vercel-app.vercel.app
```

After Render gives you the backend URL, set the frontend Vercel environment variable:

```text
VITE_API_BASE_URL=https://your-render-service.onrender.com/api
```
