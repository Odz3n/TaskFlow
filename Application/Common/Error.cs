namespace TaskFlow.Application.Common;

public record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);
    public static implicit operator string(Error error) => error.Code;
}
public record ValidationError(string PropertyName, List<string> ErrorMessages);
