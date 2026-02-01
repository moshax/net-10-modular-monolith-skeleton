using BuildingBlocks;

namespace Identity.Domain;

public sealed class User : Entity
{
    private User() { }

    public User(Guid id, string email, string passwordHash, bool isActive, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
