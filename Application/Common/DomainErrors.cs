namespace TaskFlow.Application.Common;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error NotFound = new Error("User.NotFound", "User not found");
        public static readonly Error AlreadyExists = new Error("User.AlreadyExists", "User already exists");
        public static readonly Error InvalidCredentials = new Error("User.InvalidCredentials", "Invalid email or password");
        public static readonly Error EmailNotConfirmed = new Error("User.EmailNotConfirmed", "Email not confirmed");
    }
}