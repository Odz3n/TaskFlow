namespace TaskFlow.Application.Common;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "ValidationError",
        "A validation problem occurred."
    );
    IEnumerable<Error> Errors { get; }
}