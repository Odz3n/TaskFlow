using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Responses;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Users;

public class GetUsersQuery : UserGetParameters, IQuery<PagedResult<GetUsersResponse>> 
{
    public bool IsValid => Page > 0 && PageSize > 0 && PageSize <= 100;
}