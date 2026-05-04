using System.Linq.Expressions;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Projects.Queries;

public class GetProjectsQueryHandler : IQueryHandler<GetProjectsQuery, PagedResult<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    public GetProjectsQueryHandler(
        IProjectRepository projectRepository
    )
    {
        _projectRepository = projectRepository;
    }
    public async Task<Result<PagedResult<ProjectDto>>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var sortingMap = new Dictionary<string, Expression<Func<Project, object>>>
        {
            ["name"] = p => p.Name,
            ["description"] = p => p.Description ?? "",
            ["createddate"] = p => p.CreatedDate,
            ["isarchived"] = p => p.IsArchived
        };

        var result = await _projectRepository
            .GetProjectsQueryable(cancellationToken)
            .ApplyProjectSearch(request.Parameters)
            .ApplySort(request.Parameters, sortingMap)
            .ToPagedResultAsync<Project, ProjectDto>(request.Parameters, cancellationToken);

        return Result<PagedResult<ProjectDto>>.Success(result);
    }
}