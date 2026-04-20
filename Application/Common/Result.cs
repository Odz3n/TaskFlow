namespace TaskFlow.Application.Common;

public class Result
{
    protected readonly List<Error> _errors = new();
    public Result(bool isSuccess, Error error, IEnumerable<Error>? errors = null)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;

        if (errors != null)
        {
            _errors.AddRange(errors);
        }
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public IEnumerable<Error> Errors => _errors;
    public bool HasErrors => _errors.Any();
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(Error error, IEnumerable<Error> errors) => new(false, error, errors);
    public static Result Failure(string code, string? description = null)
        => new(false, new Error(code, description));
}

public class Result<TValue>: Result
{
    protected Result(TValue? data, bool isSuccess, Error error, IEnumerable<Error>? errors = null)
        : base(isSuccess, error, errors)
    {
        Data = data;
    }
    public TValue? Data { get; }
    public static Result<TValue> Success(TValue data) 
        => new(data, true, Error.None);
    public static new Result<TValue> Failure(Error error)
        => new(default, false, error);
    public static new Result<TValue> Failure(Error error, IEnumerable<Error> errors)
        => new(default, false, error, errors);
}