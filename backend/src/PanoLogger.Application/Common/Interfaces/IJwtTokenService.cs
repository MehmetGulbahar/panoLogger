namespace PanoLogger.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(Guid userId, string username, Guid? companyId, IEnumerable<string> roles, IEnumerable<string> permissions);
}
