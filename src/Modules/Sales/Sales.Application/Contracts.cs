using Sales.Domain;

namespace Sales.Application;

public sealed record OrderDto(Guid Id, string OrderNo, string CustomerName, decimal Total, string Status, DateTimeOffset CreatedAt);

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
}

public interface ISalesUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IIdentityReadService
{
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);
}
