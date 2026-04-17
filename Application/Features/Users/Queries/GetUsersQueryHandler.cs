using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Responses;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Users;

public class GetUsersQueryHandler
    : IQueryHandler<GetUsersQuery, PagedResult<GetUsersResponse>>
{
    private readonly UserManager<User> _userManager;
    public GetUsersQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<PagedResult<GetUsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var sortMapping = new Dictionary<string, Expression<Func<User, object>>>
        {
            ["firstname"] = u => u.FirstName,
            ["lastname"] = u => u.LastName,
            ["username"] = u => u.UserName ?? "",
            ["email"] = u => u.Email ?? ""
        };

        var result = _userManager.Users
            .ApplySearch(request.SearchTerm,
                u => u.UserName,
                u => u.FirstName,
                u => u.LastName,
                u => u.Email)
            .ApplySort(request, sortMapping);

        throw new NotImplementedException();
    }
}