using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Projects.Commands;

public class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly UserManager<User> _userManager;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        UserManager<User> userManager
    )
    {
        _projectRepository = projectRepository;
        _userManager = userManager;
    }

    public async Task<Result> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var initiator = await _userManager.FindByIdAsync(request.InitiatorId.ToString() ?? Guid.Empty.ToString());
        if (initiator == null)
            return Result.Failure(new Error("User.NotFound", $"User with ID '{request.InitiatorId}' not found"));

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            CreatedDate = DateTime.UtcNow,
            IsArchived = false
        };

        var ownerMember = new Member
        {
            UserId = initiator.Id,
            Project = project,
            Role = Domain.Enums.ProjectRole.Owner,
            JoinedDate = DateTime.UtcNow,
            IsActive = true
        };

        project.Members.Add(ownerMember);

        if (request.MemberIds != null && request.MemberIds.Any())
        {
            foreach(var memberId in request.MemberIds)
            {
                if (memberId == initiator.Id)
                    continue;

                var user = await _userManager.FindByIdAsync(memberId.ToString());
                if (user == null)
                    continue;

                var member = new Member
                {
                    UserId = memberId,
                    Project = project,
                    Role = Domain.Enums.ProjectRole.Member,
                    JoinedDate = DateTime.UtcNow,
                    IsActive = true
                };

                project.Members.Add(member);
            }
        }

        await _projectRepository.AddProjectAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}