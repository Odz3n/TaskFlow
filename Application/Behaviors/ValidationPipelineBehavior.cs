using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = await ValidateAsync(request, cancellationToken);
        if (!failures.Any())
            return await next();
        return CreateValidationResult<TResponse>(failures);
    }
    private async Task<List<ValidationFailure>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
            return new List<ValidationFailure>();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        return validationResults
            .SelectMany(r => r.Errors)
            .ToList();
    }
    private TResponse CreateValidationResult<TResponse>(List<ValidationFailure> failures)
    {
        var errors = failures
            .Select(f => new Error(f.PropertyName, f.ErrorMessage))
            .ToList();

        var responseType = typeof(TResponse);

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var validationResultType = typeof(ValidationResult<>)
                .MakeGenericType(valueType);

            var method = validationResultType.GetMethod(nameof(ValidationResult<object>.WithErrors))!;
            return (TResponse)method.Invoke(null, new object[] { errors })!;
        }
        return (TResponse)(object)Common.ValidationResult.WithErrors(errors);
    }
}