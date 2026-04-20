using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Application.Common;

public class ValidationResult : Result, IValidationResult
{
    private ValidationResult(IEnumerable<Error> errors)
        : base(false, IValidationResult.ValidationError)
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        if (errors.Count() == 0)
            throw new ArgumentException("At least one error required", nameof(errors));
    }

    public IEnumerable<Error> Errors { get; }
    public static ValidationResult WithErrors(IEnumerable<Error> errors) => new(errors);
    public static ValidationResult WithError(Error error) => new(new List<Error> { error });
    public static ValidationResult FromFluentValidation(
        IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var errors = failures
            .Select(f => new Error(f.PropertyName, f.ErrorMessage))
            .ToList();

        return WithErrors(errors);
    }
    public Dictionary<string, string[]> ToDictionary()
    {
        return Errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description ?? "Unspecified")
                .ToArray()
            );
    }
    public ValidationProblemDetails ToProblemDetails()
    {
        return new ValidationProblemDetails(ToDictionary())
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Please refer to the errors property for additional details"
        };
    }
}

public class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    private ValidationResult(IEnumerable<Error> errors)
        : base(default, false, IValidationResult.ValidationError)
    {
        Errors = errors ?? throw new ArgumentNullException();
        if (!errors.Any())
            throw new ArgumentException("At least one error required", nameof(errors));
    }
    public IEnumerable<Error> Errors { get; }
    public static ValidationResult<TValue> WithErrors(IEnumerable<Error> errors)
        => new(errors);
    public static ValidationResult<TValue> WithError(Error error)
        => new(new List<Error> { error });
    public static ValidationResult<TValue> FromFluentValidation(
        IEnumerable<FluentValidation.Results.ValidationFailure> failures
    )
    {
        var errors = failures
            .Select(f => new Error(f.PropertyName, f.ErrorMessage))
            .ToList();

        return WithErrors(errors);
    }
    public static ValidationResult<TValue> FromValidationResult(ValidationResult result)
            => new(result.Errors);
    public Dictionary<string, string[]> ToDictionary()
    {
        return Errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description ?? "Unspecified").ToArray()
            );
    }
    public ValidationProblemDetails ToProblemDetails()
    {
        return new ValidationProblemDetails(ToDictionary())
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Please refer to the errors property for additional details"
        };
    }
    public static implicit operator ValidationResult<TValue>(ValidationResult result)
        => FromValidationResult(result);
    public static implicit operator ValidationResult<TValue>(Error[] errors)
        => WithErrors(errors);
}