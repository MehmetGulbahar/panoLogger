namespace PanoLogger.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles);
}
