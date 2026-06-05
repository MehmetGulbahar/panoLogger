namespace PanoLogger.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(Guid userId, string email, Guid? companyId, IEnumerable<string> roles, IEnumerable<string> permissions);
}
