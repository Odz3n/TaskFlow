using FluentValidation;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x=>x.FirstName).NotEmpty().WithMessage("First name is required").MaximumLength(50).WithMessage("First name must be less than 50 characters");
        RuleFor(x=>x.UserName).NotEmpty().WithMessage("User name is required").MaximumLength(50).WithMessage("User name must be less than 50 characters");
        RuleFor(x=>x.LastName).NotEmpty().WithMessage("Last name is required").MaximumLength(50).WithMessage("Last name  must be less than 50 characters");
        RuleFor(x=>x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email");
        RuleFor(x=>x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");
        RuleFor(x=>x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required").Equal(x=>x.Password).WithMessage("Passwords do not match");
    }
}