using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common;

namespace TaskFlow.API.Abstractions;

public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;
    protected ApiController(ISender sender)
    {
        Sender = sender;
    }
    protected IActionResult HandleFailure(Result result)
    {
        return result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(
                "Cannot handle successful result as failure"),
            IValidationResult validationResult =>
                CreateValidationProblemResponse(validationResult),
            _ => CreateDefaultErrorResponse(result)
        };
    }
    private IActionResult CreateValidationProblemResponse(IValidationResult result)
    {
        var problemDetails = new ValidationProblemDetails(
            result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description ?? "Validation error")
                    .ToArray()))
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred",
            Instance = HttpContext.Request.Path
        };

        return BadRequest(problemDetails);
    }
    private IActionResult CreateDefaultErrorResponse(Result result)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = result.Error.Description ?? "An error occurred processing your request",
            Instance = HttpContext.Request.Path,
            Extensions =
            {
                ["errorCode"] = result.Error.Code,
                ["errorDescription"] = result.Error.Description
            }
        };

        return BadRequest(problemDetails);
    }
}