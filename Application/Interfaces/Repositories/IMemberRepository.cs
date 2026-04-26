using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Repositories;

public interface IMemberRepository
{
    System.Threading.Tasks.Task AddMemberAsync(Member member, CancellationToken cancellationToken);

    IQueryable<Member> GetMembersQueryable(CancellationToken cancellationToken);
    Task<Member?> GetTrackedMemberByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<Member?> GetUntrackedMemberByIdAsync(Guid? id, CancellationToken cancellationToken);

    Task<IEnumerable<Member>> GetProjectMembersAsync(Guid? projectId, CancellationToken cancellationToken);
    Task<IEnumerable<Member>> GetAllProjectMembersAsync(Guid? projectId, CancellationToken cancellationToken);
    Task<Member?> GetProjectMemberAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken);

    Task<bool> MemberExistsAsync(Guid? id, CancellationToken cancellationToken);
    Task<bool> IsUserMemberOfProjectAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken);

    System.Threading.Tasks.Task UpdateMemberAsync(Member member, CancellationToken cancellationToken);

    System.Threading.Tasks.Task RemoveMemberAsync(Member member, CancellationToken cancellationToken);

    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken);
}