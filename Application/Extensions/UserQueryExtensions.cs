using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Extensions;

public static class UserQueryExtensions
{
    public static IQueryable<User> ApplyUserSearch(
        this IQueryable<User> query,
        UserGetParameters? parameters
    )
    {
        if (parameters == null)
            return query;

        if (parameters.UserName != null)
            query = query
                .Where(u => EF.Functions.Like(u.UserName, $"%{parameters.UserName}%"));
        if (parameters.Email != null)
            query = query
                .Where(u => EF.Functions.Like(u.Email, $"%{parameters.Email}%"));
        if (parameters.FirstName != null)
            query = query
                .Where(u => EF.Functions.Like(u.FirstName, $"%{parameters.FirstName}%"));
        if (parameters.LastName != null)
            query = query
                .Where(u => EF.Functions.Like(u.LastName, $"%{parameters.LastName}%"));

        return query;
    }
}