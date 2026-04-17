using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common;

namespace TaskFlow.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(
        RequestDelegate next
    )
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            if (context.Response.StatusCode == StatusCodes.Status400BadRequest &&
                context.Items.TryGetValue("Result", out var resultObj))
            {
                await HandleResultAsync(context, resultObj);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleResultAsync(HttpContext context, object resultObj)
    {
        var resultType = resultObj.GetType();
        var isSuccess = (bool)resultType.GetProperty("IsSuccess")?.GetValue(resultObj)!;
        if (!isSuccess)
        {
            var validationErrors = resultType.GetProperty("ValidationErrors")?.GetValue(resultObj);
            if (validationErrors != null)
            {
                var errors = (List<ValidationError>)validationErrors;
                var problemDetails = new ValidationProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = errors
                        .SelectMany(e => e.ErrorMessages)
                        .FirstOrDefault(),
                    Instance = context.Request.Path
                };

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.io/{statusCode}"
        };

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
