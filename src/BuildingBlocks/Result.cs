namespace BuildingBlocks;

public class Result
{
    protected Result(bool isSuccess, ApplicationError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ApplicationError Error { get; }

    public static Result Success() => new(true, ApplicationError.None);
    public static Result Failure(ApplicationError error) => new(false, error);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, ApplicationError error, T? value)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

#pragma warning disable CA1000 // Do not declare static members on generic types
    public static Result<T> Success(T value) => new(true, ApplicationError.None, value);
    public static new Result<T> Failure(ApplicationError error) => new(false, error, default);
#pragma warning restore CA1000 // Do not declare static members on generic types
}
