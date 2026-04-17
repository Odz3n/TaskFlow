using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens.Experimental;
using TaskFlow.Application.Common;

namespace TaskFlow.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var validationErrors = new List<Application.Common.ValidationError>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
                continue;

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator == null)
                continue;

            var validationResult = await validator
                .ValidateAsync(new ValidationContext<object>(argument));

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );

                validationErrors.Add(new Application.Common.ValidationError(
                    argument.GetType().Name,
                    errors.SelectMany(pair => pair.Value)
                        .ToList()
                ));
            }
        }
        if (validationErrors.Any())
        {
            context.Result = new BadRequestObjectResult(Result<object>.ValidationFailure(validationErrors));
            return;
        }
        await next();
    }
}