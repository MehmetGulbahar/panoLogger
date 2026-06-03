namespace PanoLogger.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    string CreateToken();
    bool ValidateToken(string token);
}
