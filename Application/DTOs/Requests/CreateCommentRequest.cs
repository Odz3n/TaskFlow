using Microsoft.AspNetCore.Http;

namespace TaskFlow.Application.DTOs.Requests;

public record CreateCommentRequest(
    string Text,
    IFormFile? File
);
