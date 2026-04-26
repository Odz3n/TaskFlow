using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    System.Threading.Tasks.Task AddTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken);

    IQueryable<Domain.Models.Task> GetTasksQueryable(CancellationToken cancellationToken);
    Task<Domain.Models.Task?> GetTrackedTaskByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<Domain.Models.Task?> GetUntrackedTaskByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.Task>> GetProjectTasksAsync(Guid? projectId, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.Task>> GetTasksByAssigneeAsync(Guid? assigneeId, CancellationToken cancellationToken);

    Task<bool> TaskExistsAsync(Guid? id, CancellationToken cancellationToken);

    System.Threading.Tasks.Task UpdateTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken);
    System.Threading.Tasks.Task RemoveTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken);
    
    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken);
}