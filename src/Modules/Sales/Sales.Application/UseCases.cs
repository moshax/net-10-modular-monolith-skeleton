using BuildingBlocks;
using FluentValidation;
using Sales.Domain;

namespace Sales.Application;

public sealed record CreateOrderCommand(string OrderNo, string CustomerName, decimal Total, string Status, Guid? CustomerId);

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.OrderNo).NotEmpty().MaximumLength(64);
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Total).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).NotEmpty().MaximumLength(40);
    }
}

public sealed class CreateOrderUseCase(
    IOrderRepository repository,
    ISalesUnitOfWork unitOfWork,
    IIdentityReadService identityReadService,
    IValidator<CreateOrderCommand> validator)
{
    public async Task<Result<OrderDto>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken).ConfigureAwait(true);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(error => error.ErrorMessage));
            return Result<OrderDto>.Failure(new ApplicationError("validation_failed", message));
        }
        ArgumentNullException.ThrowIfNull(command);
        if (command.CustomerId.HasValue)
        {
            var exists = await identityReadService.UserExistsAsync(command.CustomerId.Value, cancellationToken).ConfigureAwait(true);
            if (!exists)
            {
                return Result<OrderDto>.Failure(new ApplicationError("customer_not_found", "Customer not found."));
            }
        }

        var order = new Order(Guid.NewGuid(), command.OrderNo, command.CustomerName, command.Total, command.Status, DateTimeOffset.UtcNow);
        await repository.AddAsync(order, cancellationToken).ConfigureAwait(true);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(true);

        return Result<OrderDto>.Success(new OrderDto(order.Id, order.OrderNo, order.CustomerName, order.Total, order.Status, order.CreatedAt));
    }
}

public sealed class GetOrderUseCase(IOrderRepository repository)
{
    public async Task<Result<OrderDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(true);
        return order is null
            ? Result<OrderDto>.Failure(new ApplicationError("not_found", "Order not found."))
            : Result<OrderDto>.Success(new OrderDto(order.Id, order.OrderNo, order.CustomerName, order.Total, order.Status, order.CreatedAt));
    }
}
