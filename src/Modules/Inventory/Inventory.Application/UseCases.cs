using BuildingBlocks;
using FluentValidation;
using Inventory.Domain;

namespace Inventory.Application;

public sealed record CreateProductCommand(string Sku, string Name, int QtyOnHand);

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.QtyOnHand).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateProductUseCase(
    IProductRepository repository,
    IInventoryUnitOfWork unitOfWork,
    IValidator<CreateProductCommand> validator)
{
    public async Task<Result<ProductDto>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(error => error.ErrorMessage));
            return Result<ProductDto>.Failure(new Error("validation_failed", message));
        }

        var product = new Product(Guid.NewGuid(), command.Sku, command.Name, command.QtyOnHand, DateTimeOffset.UtcNow);
        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(new ProductDto(product.Id, product.Sku, product.Name, product.QtyOnHand, product.CreatedAt));
    }
}

public sealed class GetProductUseCase(IProductRepository repository)
{
    public async Task<Result<ProductDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null
            ? Result<ProductDto>.Failure(new Error("not_found", "Product not found."))
            : Result<ProductDto>.Success(new ProductDto(product.Id, product.Sku, product.Name, product.QtyOnHand, product.CreatedAt));
    }
}
