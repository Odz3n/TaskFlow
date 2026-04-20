using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Commands;

public class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand>
{


    public async Task<Result> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {

        return Result.Success();
    }
}