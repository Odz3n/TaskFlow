using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Tasks.Queries;

public class GetTaskByIdQueryHandler
    : IQueryHandler<GetTaskByIdQuery, TaskDetailDto>
{
    private readonly IProjectRepository  _projectRepository;
    public GetTaskByIdQueryHandler(
        IProjectRepository projectRepository
    )
    {
        _projectRepository = projectRepository;
    }
    public async Task<Result<TaskDetailDto>> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<TaskDetailDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<TaskDetailDto>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        return Result<TaskDetailDto>.Success(task.Adapt<TaskDetailDto>());
    }
}