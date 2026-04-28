using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Projects;
using TaskFlow.Application.Features.Projects.Commands;
using TaskFlow.Application.Features.Projects.Queries;

namespace MyApp.Namespace
{
    [Authorize]
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
        public async Task<IActionResult> GetProjects(
            [FromQuery] GetProjectsQuery query,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(query, cancellationToken);
            if (result.IsFailure)
                return HandleFailure(result);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(
            [FromBody] CreateProjectRequest request,
            CancellationToken ct
        )
        {
            var command = new CreateProjectCommand(
                InitiatorId: User.GetUserId(),
                InitiatorRoles: User.GetUserRoles().ToList(),
                Name: request.Name,
                Description: request.Description,
                MemberIds: request.MemberIds
            );

            var result = await _sender.Send(command, ct);
            if (result.IsFailure)
                return HandleFailure(result);
            return Ok(result);
        }
    }
}