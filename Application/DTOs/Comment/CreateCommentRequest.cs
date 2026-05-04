using Microsoft.AspNetCore.Http;

namespace TaskFlow.Application.DTOs.Comment;

public record CreateCommentRequest(
    string Text,
    IFormFile? File
);
