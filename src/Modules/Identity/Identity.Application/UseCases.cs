using BuildingBlocks;
using FluentValidation;
using Identity.Domain;

namespace Identity.Application;

public sealed record CreateUserCommand(string Email, string PasswordHash);

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PasswordHash).NotEmpty();
    }
}

public sealed class CreateUserUseCase(
    IUserRepository repository,
    IIdentityUnitOfWork unitOfWork,
    IValidator<CreateUserCommand> validator)
{
    public async Task<Result<UserDto>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(error => error.ErrorMessage));
            return Result<UserDto>.Failure(new Error("validation_failed", message));
        }

        var user = new User(Guid.NewGuid(), command.Email, command.PasswordHash, true, DateTimeOffset.UtcNow);
        await repository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UserDto>.Success(new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt));
    }
}

public sealed class GetUserUseCase(IUserRepository repository)
{
    public async Task<Result<UserDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        return user is null
            ? Result<UserDto>.Failure(new Error("not_found", "User not found."))
            : Result<UserDto>.Success(new UserDto(user.Id, user.Email, user.IsActive, user.CreatedAt));
    }
}
