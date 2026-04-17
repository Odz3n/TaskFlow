namespace TaskFlow.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<ValidationError>? ValidationErrors { get; }
    private Result()
    {
        IsSuccess = true;
    }
    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }
    private Result(List<ValidationError> errors)
    {
        IsSuccess = false;
        ValidationErrors = errors;
    }
    public static Result Success() => new();
    public static Result Failure(string error) => new(error);
    public static Result ValidationFailure(List<ValidationError> errors) => new(errors);
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public List<ValidationError>? ValidationErrors { get; }
    private Result(T data)
    {
        IsSuccess = true;
        Data = data;
    }
    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }
    private Result(List<ValidationError> errors)
    {
        IsSuccess = false;
        ValidationErrors = errors;
    }
    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failure(string error) => new(error);
    public static Result<T> ValidationFailure(List<ValidationError> errors) => new(errors);
}

public record ValidationError(string PropertyName, List<string> ErrorMessages);