using System.Security.Cryptography;
using PanoLogger.Application.Common.Interfaces;

namespace PanoLogger.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string CreateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public bool ValidateToken(string token)
    {
        return !string.IsNullOrWhiteSpace(token);
    }
}
