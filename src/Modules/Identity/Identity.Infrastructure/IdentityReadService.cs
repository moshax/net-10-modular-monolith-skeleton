using Microsoft.EntityFrameworkCore;
using Sales.Application;

namespace Identity.Infrastructure;

public sealed class IdentityReadService(IdentityDbContext dbContext) : IIdentityReadService
{
    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId, cancellationToken);
    }
}
