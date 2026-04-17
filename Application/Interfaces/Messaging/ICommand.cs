using MediatR;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Interfaces.Messaging;

public interface ICommand: IRequest<Result>
{
}

public interface ICommand<TResponse>: IRequest<Result<TResponse>>
{
}