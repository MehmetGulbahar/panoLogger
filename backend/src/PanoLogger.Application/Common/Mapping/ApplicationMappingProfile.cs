using AutoMapper;
using PanoLogger.Domain.Companies;
using PanoLogger.Domain.Facilities;
using PanoLogger.Domain.Files;
using PanoLogger.Domain.Panels;
using PanoLogger.Domain.Users;

namespace PanoLogger.Application.Common.Mapping;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Company, CompanyReadModel>();
        CreateMap<Facility, FacilityReadModel>();
        CreateMap<Panel, PanelReadModel>();
        CreateMap<PanelFile, PanelFileReadModel>();
        CreateMap<User, UserReadModel>();
    }
}

public sealed record CompanyReadModel(Guid Id, string Name, string ProjectName, string TaxNumber, string Address, string ContactEmail);
public sealed record FacilityReadModel(Guid Id, Guid CompanyId, string Name, string City, string Address);
public sealed record PanelReadModel(Guid Id, Guid FacilityId, string Code, string Name, string Description);
public sealed record PanelFileReadModel(Guid Id, Guid PanelId, string Category, string FileName, string StoragePath, string ContentType, long SizeBytes);
public sealed record UserReadModel(Guid Id, string Email, string DisplayName, bool IsActive);
