namespace BuildingBlocks;

public sealed record ApplicationError(string Code, string Message)
{
    public static readonly ApplicationError None = new(string.Empty, string.Empty);
}
