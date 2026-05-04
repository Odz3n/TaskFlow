using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Extensions;

public static class MemberQueryExtensions
{
    public static IQueryable<Member> ApplyMemberSearch(
        this IQueryable<Member> query,
        MemberGetParameters? parameters
    )
    {
        if (parameters == null)
            return query;
        
        if (parameters.UserName != null)
            query = query
                .Where(m => EF.Functions.Like(m.User.UserName, $"%{parameters.UserName}%"));
        if (parameters.ProjectRole != null)
            query = query
                .Where(m => EF.Functions.Like(m.Role.ToString(), $"%{parameters.ProjectRole}%"));
        if (parameters.JoinedDate != null)
            query = query
                .Where(m => EF.Functions.Like(m.JoinedDate.ToShortDateString(), $"%{parameters.JoinedDate}%"));
        if (parameters.IsActive != null)
            query = query
                .Where(m => EF.Functions.Like(m.IsActive.ToString(), $"%{parameters.IsActive}%"));
        return query;
    }
}