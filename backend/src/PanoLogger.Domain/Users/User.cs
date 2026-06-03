using PanoLogger.Domain.Common;

namespace PanoLogger.Domain.Users;

public sealed class User : Entity
{
    public required string Email { get; init; }
    public required string DisplayName { get; init; }
    public string PasswordHash { get; private set; } = "";
    public bool IsActive { get; init; } = true;

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}
