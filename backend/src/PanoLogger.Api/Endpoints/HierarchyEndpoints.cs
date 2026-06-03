using Microsoft.EntityFrameworkCore;
using PanoLogger.Api.Authorization;
using PanoLogger.Application.Common.Exceptions;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Domain.Companies;
using PanoLogger.Domain.Facilities;
using PanoLogger.Domain.Panels;
using PanoLogger.Infrastructure.Persistence;

namespace PanoLogger.Api.Endpoints;

public static class HierarchyEndpoints
{
    public static IEndpointRouteBuilder MapHierarchyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hierarchy")
            .WithTags("Hierarchy")
            .RequireAuthorization();

        group.MapGet("/", async (PanoLoggerDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var companies = await dbContext.Companies.AsNoTracking().OrderBy(item => item.ProjectName).ToListAsync(cancellationToken);
            var facilities = await dbContext.Facilities.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);
            var panels = await dbContext.Panels.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

            return Results.Ok(companies.Select(company => new HierarchyCompanyResponse(
                company.Id,
                company.Name,
                company.ProjectName,
                company.TaxNumber,
                company.Address,
                company.ContactEmail,
                facilities
                    .Where(facility => facility.CompanyId == company.Id)
                    .Select(facility => new HierarchyFacilityResponse(
                        facility.Id,
                        facility.CompanyId,
                        facility.Name,
                        facility.City,
                        facility.District,
                        facility.Address,
                        panels
                            .Where(panel => panel.FacilityId == facility.Id)
                            .Select(panel => new HierarchyPanelResponse(
                                panel.Id,
                                panel.FacilityId,
                                panel.Code,
                                panel.Name,
                                panel.Description))
                            .ToArray()))
                    .ToArray())));
        })
        .WithName("GetHierarchy");

        group.MapPost("/systems", async (
            CreateSystemRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            ValidateRequest(request);

            var taxNumber = request.TaxNumber.Trim();
            var contactEmail = request.ContactEmail.Trim().ToLowerInvariant();
            var panelCode = request.PanelCode.Trim().ToUpperInvariant();

            if (await dbContext.Companies.AnyAsync(item => item.TaxNumber == taxNumber, cancellationToken))
            {
                throw new ValidationException("Bu vergi numarası ile kayıtlı bir şirket zaten var.");
            }

            if (await dbContext.Panels.AnyAsync(item => item.Code == panelCode, cancellationToken))
            {
                throw new ValidationException("Bu pano kodu zaten kullanılıyor.");
            }

            var company = new Company
            {
                Name = request.CompanyName.Trim(),
                ProjectName = request.ProjectName.Trim(),
                TaxNumber = taxNumber,
                Address = request.CompanyAddress.Trim(),
                ContactEmail = contactEmail,
            };
            var facility = new Facility
            {
                CompanyId = company.Id,
                Name = request.FacilityName.Trim(),
                City = request.City.Trim(),
                District = request.District.Trim(),
                Address = request.FacilityAddress.Trim(),
            };
            var panel = new Panel
            {
                FacilityId = facility.Id,
                Code = panelCode,
                Name = request.PanelName.Trim(),
                Description = request.PanelDescription.Trim(),
            };

            dbContext.Companies.Add(company);
            dbContext.Facilities.Add(facility);
            dbContext.Panels.Add(panel);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created(
                $"/api/companies/{company.Id}",
                new HierarchyCompanyResponse(
                    company.Id,
                    company.Name,
                    company.ProjectName,
                    company.TaxNumber,
                    company.Address,
                    company.ContactEmail,
                    [
                        new HierarchyFacilityResponse(
                            facility.Id,
                            facility.CompanyId,
                            facility.Name,
                            facility.City,
                            facility.District,
                            facility.Address,
                            [
                                new HierarchyPanelResponse(
                                    panel.Id,
                                    panel.FacilityId,
                                    panel.Code,
                                    panel.Name,
                                    panel.Description)
                            ])
                    ]));
        })
        .RequireAuthorization(AuthorizationPolicies.ManageCompanies)
        .WithName("CreateSystem");

        group.MapPut("/companies/{companyId:guid}", async (
            Guid companyId,
            UpdateCompanyRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            ValidateCompanyRequest(request.ProjectName, request.CompanyName, request.TaxNumber, request.Address, request.ContactEmail);

            var taxNumber = request.TaxNumber.Trim();
            var contactEmail = request.ContactEmail.Trim().ToLowerInvariant();

            if (await dbContext.Companies.AnyAsync(
                item => item.Id != companyId && item.TaxNumber == taxNumber,
                cancellationToken))
            {
                throw new ValidationException("Bu vergi numarası ile kayıtlı bir şirket zaten var.");
            }

            var updatedRows = await dbContext.Companies
                .Where(item => item.Id == companyId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(item => item.ProjectName, request.ProjectName.Trim())
                        .SetProperty(item => item.Name, request.CompanyName.Trim())
                        .SetProperty(item => item.TaxNumber, taxNumber)
                        .SetProperty(item => item.Address, request.Address.Trim())
                        .SetProperty(item => item.ContactEmail, contactEmail)
                        .SetProperty(item => item.UpdatedAtUtc, DateTimeOffset.UtcNow),
                    cancellationToken);

            if (updatedRows == 0)
            {
                throw new NotFoundException($"Company '{companyId}' was not found.");
            }

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageCompanies)
        .WithName("UpdateCompany");

        group.MapDelete("/companies/{companyId:guid}", async (
            Guid companyId,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var company = await dbContext.Companies.FirstOrDefaultAsync(item => item.Id == companyId, cancellationToken)
                ?? throw new NotFoundException($"Company '{companyId}' was not found.");

            var storagePaths = await (
                from facility in dbContext.Facilities.AsNoTracking()
                join panel in dbContext.Panels.AsNoTracking() on facility.Id equals panel.FacilityId
                join file in dbContext.PanelFiles.AsNoTracking() on panel.Id equals file.PanelId
                where facility.CompanyId == companyId
                select file.StoragePath
            ).ToListAsync(cancellationToken);

            foreach (var storagePath in storagePaths)
            {
                await fileStorageService.DeleteAsync(storagePath, cancellationToken);
            }

            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageCompanies)
        .WithName("DeleteCompany");

        group.MapPut("/facilities/{facilityId:guid}", async (
            Guid facilityId,
            UpdateFacilityRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (new[] { request.Name, request.City, request.District, request.Address }.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidationException("Tüm tesis alanları zorunludur.");
            }

            var name = request.Name.Trim();
            var facility = await dbContext.Facilities
                .AsNoTracking()
                .Where(item => item.Id == facilityId)
                .Select(item => new { item.CompanyId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Facility '{facilityId}' was not found.");

            if (await dbContext.Facilities.AnyAsync(
                item => item.Id != facilityId && item.CompanyId == facility.CompanyId && item.Name == name,
                cancellationToken))
            {
                throw new ValidationException("Bu şirket altında aynı isimde bir tesis zaten var.");
            }

            await dbContext.Facilities
                .Where(item => item.Id == facilityId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(item => item.Name, name)
                        .SetProperty(item => item.City, request.City.Trim())
                        .SetProperty(item => item.District, request.District.Trim())
                        .SetProperty(item => item.Address, request.Address.Trim())
                        .SetProperty(item => item.UpdatedAtUtc, DateTimeOffset.UtcNow),
                    cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFacilities)
        .WithName("UpdateFacility");

        group.MapPost("/facilities", async (
            CreateFacilityRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (new[] { request.Name, request.City, request.District, request.Address }.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidationException("Tüm tesis alanları zorunludur.");
            }

            if (!await dbContext.Companies.AnyAsync(item => item.Id == request.CompanyId, cancellationToken))
            {
                throw new NotFoundException($"Company '{request.CompanyId}' was not found.");
            }

            var name = request.Name.Trim();
            if (await dbContext.Facilities.AnyAsync(
                item => item.CompanyId == request.CompanyId && item.Name == name,
                cancellationToken))
            {
                throw new ValidationException("Bu şirket altında aynı isimde bir tesis zaten var.");
            }

            var facility = new Facility
            {
                CompanyId = request.CompanyId,
                Name = name,
                City = request.City.Trim(),
                District = request.District.Trim(),
                Address = request.Address.Trim(),
            };

            dbContext.Facilities.Add(facility);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created(
                $"/api/facilities/{facility.Id}",
                new HierarchyFacilityResponse(
                    facility.Id,
                    facility.CompanyId,
                    facility.Name,
                    facility.City,
                    facility.District,
                    facility.Address,
                    []));
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFacilities)
        .WithName("CreateFacility");

        group.MapDelete("/facilities/{facilityId:guid}", async (
            Guid facilityId,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var facility = await dbContext.Facilities.FirstOrDefaultAsync(item => item.Id == facilityId, cancellationToken)
                ?? throw new NotFoundException($"Facility '{facilityId}' was not found.");

            var storagePaths = await (
                from panel in dbContext.Panels.AsNoTracking()
                join file in dbContext.PanelFiles.AsNoTracking() on panel.Id equals file.PanelId
                where panel.FacilityId == facilityId
                select file.StoragePath
            ).ToListAsync(cancellationToken);

            foreach (var storagePath in storagePaths)
            {
                await fileStorageService.DeleteAsync(storagePath, cancellationToken);
            }

            dbContext.Facilities.Remove(facility);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManageFacilities)
        .WithName("DeleteFacility");

        group.MapPut("/panels/{panelId:guid}", async (
            Guid panelId,
            UpdatePanelRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (new[] { request.Code, request.Name, request.Description }.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidationException("Tüm pano alanları zorunludur.");
            }

            var code = request.Code.Trim().ToUpperInvariant();
            var name = request.Name.Trim();
            var panel = await dbContext.Panels
                .AsNoTracking()
                .Where(item => item.Id == panelId)
                .Select(item => new { item.FacilityId })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Panel '{panelId}' was not found.");

            if (await dbContext.Panels.AnyAsync(item => item.Id != panelId && item.Code == code, cancellationToken))
            {
                throw new ValidationException("Bu pano kodu zaten kullanılıyor.");
            }

            if (await dbContext.Panels.AnyAsync(
                item => item.Id != panelId && item.FacilityId == panel.FacilityId && item.Name == name,
                cancellationToken))
            {
                throw new ValidationException("Bu tesis altında aynı isimde bir pano zaten var.");
            }

            await dbContext.Panels
                .Where(item => item.Id == panelId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(item => item.Code, code)
                        .SetProperty(item => item.Name, name)
                        .SetProperty(item => item.Description, request.Description.Trim())
                        .SetProperty(item => item.UpdatedAtUtc, DateTimeOffset.UtcNow),
                    cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManagePanels)
        .WithName("UpdatePanel");

        group.MapPost("/panels", async (
            CreatePanelRequest request,
            PanoLoggerDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (new[] { request.Code, request.Name, request.Description }.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidationException("Tüm pano alanları zorunludur.");
            }

            if (!await dbContext.Facilities.AnyAsync(
                item => item.Id == request.FacilityId && item.CompanyId == request.CompanyId,
                cancellationToken))
            {
                throw new NotFoundException("Seçilen tesis bu şirkete bağlı değil.");
            }

            var code = request.Code.Trim().ToUpperInvariant();
            var name = request.Name.Trim();
            if (await dbContext.Panels.AnyAsync(item => item.Code == code, cancellationToken))
            {
                throw new ValidationException("Bu pano kodu zaten kullanılıyor.");
            }

            if (await dbContext.Panels.AnyAsync(
                item => item.FacilityId == request.FacilityId && item.Name == name,
                cancellationToken))
            {
                throw new ValidationException("Bu tesis altında aynı isimde bir pano zaten var.");
            }

            var panel = new Panel
            {
                FacilityId = request.FacilityId,
                Code = code,
                Name = name,
                Description = request.Description.Trim(),
            };

            dbContext.Panels.Add(panel);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created(
                $"/api/panels/{panel.Id}",
                new HierarchyPanelResponse(panel.Id, panel.FacilityId, panel.Code, panel.Name, panel.Description));
        })
        .RequireAuthorization(AuthorizationPolicies.ManagePanels)
        .WithName("CreatePanel");

        group.MapDelete("/panels/{panelId:guid}", async (
            Guid panelId,
            PanoLoggerDbContext dbContext,
            IFileStorageService fileStorageService,
            CancellationToken cancellationToken) =>
        {
            var panel = await dbContext.Panels.FirstOrDefaultAsync(item => item.Id == panelId, cancellationToken)
                ?? throw new NotFoundException($"Panel '{panelId}' was not found.");

            var storagePaths = await dbContext.PanelFiles
                .AsNoTracking()
                .Where(file => file.PanelId == panelId)
                .Select(file => file.StoragePath)
                .ToListAsync(cancellationToken);

            foreach (var storagePath in storagePaths)
            {
                await fileStorageService.DeleteAsync(storagePath, cancellationToken);
            }

            dbContext.Panels.Remove(panel);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.ManagePanels)
        .WithName("DeletePanel");

        return app;
    }

    private static void ValidateRequest(CreateSystemRequest request)
    {
        var requiredValues = new[]
        {
            request.ProjectName,
            request.CompanyName,
            request.TaxNumber,
            request.CompanyAddress,
            request.ContactEmail,
            request.FacilityName,
            request.City,
            request.District,
            request.FacilityAddress,
            request.PanelCode,
            request.PanelName,
            request.PanelDescription,
        };

        if (requiredValues.Any(string.IsNullOrWhiteSpace))
        {
            throw new ValidationException("Tüm sistem alanları zorunludur.");
        }

        if (!request.ContactEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ValidationException("Geçerli bir iletişim e-postası girin.");
        }
    }

    private static void ValidateCompanyRequest(
        string projectName,
        string companyName,
        string taxNumber,
        string address,
        string contactEmail)
    {
        if (new[] { projectName, companyName, taxNumber, address, contactEmail }.Any(string.IsNullOrWhiteSpace))
        {
            throw new ValidationException("Tüm şirket alanları zorunludur.");
        }

        if (!contactEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ValidationException("Geçerli bir iletişim e-postası girin.");
        }
    }
}

public sealed record CreateSystemRequest(
    string ProjectName,
    string CompanyName,
    string TaxNumber,
    string CompanyAddress,
    string ContactEmail,
    string FacilityName,
    string City,
    string District,
    string FacilityAddress,
    string PanelCode,
    string PanelName,
    string PanelDescription);

public sealed record UpdateCompanyRequest(
    string ProjectName,
    string CompanyName,
    string TaxNumber,
    string Address,
    string ContactEmail);

public sealed record UpdateFacilityRequest(string Name, string City, string District, string Address);

public sealed record CreateFacilityRequest(Guid CompanyId, string Name, string City, string District, string Address);

public sealed record UpdatePanelRequest(string Code, string Name, string Description);

public sealed record CreatePanelRequest(Guid CompanyId, Guid FacilityId, string Code, string Name, string Description);

public sealed record HierarchyCompanyResponse(
    Guid Id,
    string Name,
    string ProjectName,
    string TaxNumber,
    string Address,
    string ContactEmail,
    IReadOnlyCollection<HierarchyFacilityResponse> Facilities);

public sealed record HierarchyFacilityResponse(
    Guid Id,
    Guid CompanyId,
    string Name,
    string City,
    string District,
    string Address,
    IReadOnlyCollection<HierarchyPanelResponse> Panels);

public sealed record HierarchyPanelResponse(
    Guid Id,
    Guid FacilityId,
    string Code,
    string Name,
    string Description);
