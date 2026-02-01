using Identity.Domain;

namespace Identity.Application;

public sealed record UserDto(Guid Id, string Email, bool IsActive, DateTimeOffset CreatedAt);

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}

public interface IIdentityUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
