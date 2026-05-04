using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Extensions;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(
            Guid id,
            CancellationToken ct
        )
        {
            var query = new GetProjectByIdQuery(id);
            var result = await _sender.Send(query, ct);
            
            if (result.IsFailure)
                return HandleFailure(result);
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            Guid id,
            [FromBody] UpdateProjectRequest request,
            CancellationToken ct
        )
        {
            var command = new UpdateProjectCommand(
                Id: id,
                InitiatorId: User.GetUserId(),
                Name: request.Name,
                Description: request.Description,
                IsArchived: request.IsArchived
            );

            var result = await _sender.Send(command, ct);
            
            if (result.IsFailure)
                return HandleFailure(result);
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(
            Guid id,
            CancellationToken ct
        )
        {
            var command = new DeleteProjectCommand(
                Id: id,
                InitiatorId: User.GetUserId()
            );

            var result = await _sender.Send(command, ct);
            
            if (result.IsFailure)
                return HandleFailure(result);
            return NoContent();
        }
    }
}