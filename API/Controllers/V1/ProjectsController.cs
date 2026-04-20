using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Features.Projects;
using TaskFlow.Application.Features.Projects.Commands;

namespace MyApp.Namespace
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProjectsController : ApiController
    {
        private readonly ISender _sender;

        public ProjectsController(ISender sender)
            : base(sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            return Ok(123);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(
            [FromForm] CreateProjectRequest request,
            CancellationToken ct
        )
        {
            var command = new CreateProjectCommand(
                request.Name,
                request.Description,
                request.MemberIds);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }
    }
}