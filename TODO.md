# TODO Checklist

## Phase 1 - Frontend Foundation

- [x] Create Vue 3 Project ✅
- [x] Configure TypeScript ✅
- [x] Configure PrimeVue ✅
- [x] Configure PrimeIcons ✅
- [x] Configure PrimeFlex ✅
- [x] Configure Vue Router ✅
- [x] Configure Pinia ✅
- [x] Configure Axios ✅
- [x] Configure TanStack Query ✅
- [x] Configure ESLint ✅
- [x] Configure Prettier ✅
- [x] Configure Environment Variables ✅
- [x] Configure API Layer ✅
- [x] Configure Service Layer ✅
- [x] Configure Store Layer ✅
- [x] Configure Composables Layer ✅
- [x] Configure Validators Layer ✅
- [x] Configure Types Layer ✅
- [x] Configure Utilities Layer ✅
- [x] Configure Layout Architecture ✅
- [x] Configure Responsive Architecture ✅
- [x] Configure Theme Architecture ✅
- [x] Configure Enterprise Folder Structure ✅
- [x] Create Initial Application Layout ✅
- [x] Create Sidebar ✅
- [x] Create Header ✅
- [x] Create Breadcrumb System ✅
- [x] Create Global Loading Infrastructure ✅
- [x] Create Global Error Infrastructure ✅

## Phase 2 - Design System

- [x] Design Tokens ✅
- [x] Typography System ✅
- [x] Color System ✅
- [x] Spacing System ✅
- [x] Card Components ✅
- [x] Button Components ✅
- [x] Table Components ✅
- [x] Form Components ✅
- [x] Modal Components ✅
- [ ] Confirmation Dialog Components
- [x] Empty State Components ✅
- [x] Loading Components ✅
- [x] Error Components ✅
- [x] Mobile Components ✅
- [x] Responsive Components ✅

## Phase 3 - Application Pages

- [x] Login ✅
- [x] Dashboard ✅
- [x] Companies List ✅
- [x] Company Detail ✅
- [x] Facilities List ✅
- [x] Facility Detail ✅
- [x] Panels List ✅
- [x] Panel Detail ✅

## Phase 4 - Public Mobile QR Page

- [x] Public Route /p/{panelCode} ✅
- [x] Panel Name ✅
- [x] Electrical Project Card ✅
- [x] Maintenance Report Card ✅
- [x] Panel Documents Card ✅
- [x] Download Buttons ✅

## Phase 5 - Backend Foundation

- [x] Create Clean Architecture Solution ✅
- [x] Create API Layer ✅
- [x] Create Application Layer ✅
- [x] Create Domain Layer ✅
- [x] Create Infrastructure Layer ✅
- [x] Configure MediatR ✅
- [x] Configure FluentValidation ✅
- [x] Configure AutoMapper ✅
- [x] Configure Serilog ✅
- [x] Configure Swagger ✅
- [x] Configure Exception Middleware ✅
- [x] Configure Logging ✅
- [x] Configure Audit Logging ✅
- [x] Configure JWT Authentication ✅
- [x] Configure Refresh Tokens ✅

## Phase 6 - Database Design

- [x] Companies
- [x] Facilities
- [x] Panels
- [x] Files
- [x] MaintenanceReports
- [x] Users
- [x] Roles
- [x] UserRoles
- [x] AuditLogs
- [x] ER Diagram
- [x] Entity Configurations
- [x] Migrations

## Phase 7 - Supabase Integration

- [x] PostgreSQL Integration
- [x] Supabase Storage Integration
- [x] Upload Service
- [x] Download Service
- [x] Delete Service
- [x] Signed URL Support
- [x] File Security

## Phase 8 - QR Management

- [x] Generate QR Codes
- [x] Unique Panel Codes
- [x] Public Access Endpoint
- [x] QR Download
- [x] QR Print Support
- [x] QR Regeneration

## Phase 9 - Authorization

- [x] Super Admin
- [x] Company Admin
- [x] Facility Manager
- [x] Viewer
- [x] Role Based Access Control
- [x] Permission Management
- [x] Endpoint Authorization
- [x] UI Authorization

## Phase 10 - Finalization

- [ ] Docker Support
- [ ] CI/CD Pipeline
- [ ] Production Configuration
- [ ] Security Review
- [ ] Performance Optimization
- [ ] Database Optimization
- [ ] API Documentation
- [ ] User Documentation
- [ ] Deployment Documentation
- [ ] Production Readiness Checklist

## Recent Completed Work

- [x] Replaced tax-number login with company-code account binding while keeping server-side company tenant isolation
- [x] Added company tenant isolation with user-company linkage, scoped hierarchy/file/QR APIs, and tenant-aware auth responses
- [x] Added website-matched success toast notifications for login and registration
- [x] Replaced dashboard mock counters and fixed upload summary with authenticated API-backed hierarchy and file statistics
- [x] Replaced the hardcoded facility district with a real database-backed district field in create, edit, list, and detail views
- [x] Added company-aware create forms for facilities and panels, including facility filtering by selected company
- [x] Added role-aware facility and panel edit/delete actions with cascade file cleanup
- [x] Added SuperAdmin company edit and cascade delete actions with compact table controls and confirmation
- [x] Replaced visible hierarchy mock data with authenticated API data and added a compact transactional New System form
- [x] Activated panel file upload with Supabase Storage API integration, file metadata persistence, signed downloads, and stable demo panel IDs
- [x] Allowed Viewer accounts to access company, facility, and panel pages in read-only mode while hiding management actions

- [x] Moved frontend authentication from local storage bearer tokens to an HttpOnly JWT cookie with cookie session hydration and logout support ✅
- [x] Added real frontend registration and login flow with backend user creation, password hashing, default Viewer role assignment, and CORS support ✅
- [x] Completed Phase 9 authorization with seeded roles, role policies, permission mapping, protected QR endpoints, auth permissions endpoint, frontend auth store, route guards, and login token flow ✅
- [x] Completed Phase 8 QR management with QR SVG generation, unique panel code service, public panel access endpoint, QR download, print view, and regeneration endpoint ✅
- [x] Completed Phase 7 Supabase integration with Postgres config override, storage REST client, upload/download/delete/signed URL services, and file security validation ✅
- [x] Completed Phase 6 database design with EF Core entities, PostgreSQL DbContext, entity configurations, snake_case initial migration, and ER diagram ✅
- [x] Added mock project names for AVM Elektrik Yönetimi and İşyeri Elektrik Yönetimi ✅
- [x] Wired project names into the companies table, company detail page, and sidebar hierarchy ✅
- [x] Fixed sidebar hierarchy rendering from mock companies/facilities/panels ✅
- [x] Added mock-data.js compatibility bridge for Vite dev server module resolution ✅
- [x] Matched project text and icon scale closer to the original reference across header, sidebar, footer/mobile controls, dashboard, company detail, facility detail, and panel detail ✅
- [x] Replaced oversized emoji UI icons with PrimeIcons where relevant ✅
- [x] Verified production build with npx vite build ✅
- [x] Built public mobile QR page for /p/{panelCode} with standalone layout and mock download buttons ✅
- [x] Scaffolded .NET Clean Architecture backend solution under backend/ ✅
- [x] Added API/Application/Domain/Infrastructure projects and references ✅
- [x] Added health endpoint, exception middleware, domain entities, audit logging contract, JWT options, and refresh token service ✅
- [x] Verified backend build with dotnet build backend/PanoLogger.Backend.slnx --no-restore ✅
- [x] Wired MediatR, FluentValidation pipeline behavior, AutoMapper profile, Serilog, Swagger UI, and JWT bearer authentication ✅
- [x] Added development token endpoint and protected current-user endpoint for auth verification ✅
- [x] Verified backend Release build with dotnet build backend/PanoLogger.Backend.slnx -c Release --no-restore ✅
- [x] Built compact standalone login page with real form controls and demo submit flow ✅
