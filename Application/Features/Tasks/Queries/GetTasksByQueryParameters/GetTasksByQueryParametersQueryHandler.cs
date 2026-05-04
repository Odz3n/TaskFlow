using System.Linq.Expressions;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Tasks.Queries;

public class GetTasksByQueryParametersQueryHandler
    : IQueryHandler<GetTasksByQueryParametersQuery, PagedResult<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    public GetTasksByQueryParametersQueryHandler(
        ITaskRepository taskRepository
    )
    {
        _taskRepository = taskRepository;
    }
    public async Task<Result<PagedResult<TaskDto>>> Handle(GetTasksByQueryParametersQuery request, CancellationToken cancellationToken)
    {
        var sortingMap = new Dictionary<string, Expression<Func<Domain.Models.Task, object>>>
        {
            ["title"] = t => t.Title,
            ["description"] = t => t.Description??"",
            ["status"] = t => t.Status,
            ["priority"] = t => t.Priority,
            ["duedate"] = t => t.DueDate,
            ["createddate"] = t => t.CreatedDate,
            ["updateddate"] = t => t.UpdatedDate
        };

        var result = await _taskRepository
            .GetTasksQueryable(cancellationToken)
            .Where(t => t.ProjectId == request.ProjectId)
            .ApplyTaskSearch(request.Parameters)
            .ApplySort(request.Parameters, sortingMap)
            .ToPagedResultAsync<Domain.Models.Task, TaskDto>(request.Parameters, cancellationToken);

        return Result<PagedResult<TaskDto>>.Success(result);
    }
}