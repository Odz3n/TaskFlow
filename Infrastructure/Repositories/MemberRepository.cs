using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _db;
    public MemberRepository(
        AppDbContext db
    )
    {
        _db = db;
    }
    public async System.Threading.Tasks.Task AddMemberAsync(Member member, CancellationToken cancellationToken)
    {
        await _db.Members.AddAsync(member, cancellationToken);
    }

    public IQueryable<Member> GetMembersQueryable(CancellationToken cancellationToken)
    {
        return GetMembersWithIncludes(asNoTracking: true);
    }

    public async Task<Member?> GetProjectMemberAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken)
    {
        return await GetMembersWithIncludes(asNoTracking: true)
            .FirstOrDefaultAsync(m => m.ProjectId == projectId &&
                                 m.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Member>> GetProjectMembersAsync(Guid? projectId, CancellationToken cancellationToken)
    {
        return await GetMembersWithIncludes(asNoTracking: true)
            .Where(m => m.ProjectId == projectId && m.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Member>> GetAllProjectMembersAsync(Guid? projectId, CancellationToken cancellationToken)
    {
        return await GetMembersWithIncludes(asNoTracking: true)
            .Where(m => m.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Member?> GetTrackedMemberByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetMembersWithIncludes(asNoTracking: false)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Member?> GetUntrackedMemberByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetMembersWithIncludes(asNoTracking: true)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> IsUserMemberOfProjectAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken)
    {
        return await _db.Members
            .AnyAsync(m => m.UserId == userId &&
                      m.ProjectId == projectId &&
                      m.IsActive, cancellationToken);
    }

    public async Task<bool> MemberExistsAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await _db.Members
            .AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveMemberAsync(Member member, CancellationToken cancellationToken)
    {
        _db.Members.Remove(member);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateMemberAsync(Member member, CancellationToken cancellationToken)
    {
        _db.Members.Update(member);
    }
    private IQueryable<Member> GetMembersWithIncludes(
        bool asNoTracking = false
    )
    {
        var query = _db.Members
            .Include(m => m.User)
            .Include(m => m.Project)
            .Include(m => m.AssignedTasks)
            .Include(m => m.CreatedTasks)
            .Include(m => m.Comments)
            .Include(m => m.Attachments);

        return asNoTracking ? query.AsNoTracking() : query;
    }
}