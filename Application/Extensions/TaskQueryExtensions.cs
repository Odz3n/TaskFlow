using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.Task;

namespace TaskFlow.Application.Extensions;

public static class TaskQueryExtensions
{
    public static IQueryable<Domain.Models.Task> ApplyTaskSearch(
        this IQueryable<Domain.Models.Task> query,
        TaskGetParameters? parameters
    )
    {
        if (parameters == null)
            return query;

        if (parameters.Title != null)
            query = query
                .Where(t => EF.Functions.Like(t.Title, $"%{parameters.Title}%"));
        if (parameters.Description != null)
            query = query
                .Where(t => EF.Functions.Like(t.Description, $"%{parameters.Description}%"));
        if (parameters.AssigneeMemberId != null)
            query = query
                .Where(t => EF.Functions.Like(t.AssigneeMemberId.ToString(), $"%{parameters.AssigneeMemberId}%"));
        if (parameters.Status != null)
            query = query
                .Where(t => EF.Functions.Like(t.Status.ToString(), $"%{parameters.Status}%"));
        if (parameters.Priority != null)
            query = query
                .Where(t => EF.Functions.Like(t.Priority.ToString(), $"%{parameters.Priority}%"));
        if (parameters.CreatedDate != null)
            query = query
                .Where(t => EF.Functions.Like(t.CreatedDate.ToShortDateString(), $"%{parameters.CreatedDate}%"));
        if (parameters.DueDate != null)
            query = query
                .Where(t => EF.Functions.Like(t.DueDate.ToShortDateString(), $"%{parameters.DueDate}%"));
        
        return query;
    }
}