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

    public static class Project
    {
        public static readonly Error NotFound = new Error("Project.NotFound", "Project not found");
        public static readonly Error PermissionDenied = new Error("Project.PermissionDenied", "You do not have permission to perform this action");
    }

    public static class Task
    {
        public static readonly Error NotFound = new Error("Task.NotFound", "Task not found");
    }

    public static class Comment
    {
        public static readonly Error NotFound = new Error("Comment.NotFound", "Comment not found");
        public static readonly Error PermissionDenied = new Error("Comment.PermissionDenied", "You do not have permission to modify this comment");
    }
}