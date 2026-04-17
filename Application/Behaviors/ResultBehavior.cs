using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens.Experimental;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Behaviors;

public class ResultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ResultBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var errors = failures
                    .GroupBy(e => e.PropertyName)
                    .Select(g => new Common.ValidationError(
                        g.Key,
                        g.Select(e => e.ErrorMessage)
                        .ToList()
                    ))
                    .ToList();

                var resultType = typeof(Result<>).MakeGenericType(
                    typeof(TResponse).GetGenericArguments()[0]);

                var validationFailure = resultType.GetMethod("ValidationFailure")
                ?.Invoke(null, new object[] { errors });

                return (TResponse)validationFailure!;
            }
        }
        return await next();
    }
}