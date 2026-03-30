using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.Roles;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Update;

internal sealed class UpdateClubMemberHandler : IRequestHandler<UpdateClubMember, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IIdentityService _identityService;

    public UpdateClubMemberHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository,  IUserContext userContext, IClubMemberQueries clubMemberQueries,  IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdateClubMember request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure(ClubErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<int>(ClubMemberErrors.InvalidType);
        }
        
        var currentMember = club.Members.FirstOrDefault(m => m.UserId == request.UserId);
        var oldRole = currentMember?.Role;

        var memberResult = club.UpdateMember(request.UserId, memberRole, request.IsMember, request.MembershipNumber);
        if (memberResult.IsFailure)
        {
            return Result.Failure(memberResult.Error);
        }

        bool isManagementRole(MemberRole? role) => role == MemberRole.Admin || role == MemberRole.Owner || role == MemberRole.Coach;
        bool needsPrivileges = isManagementRole(memberRole);
        bool hadPrivileges = isManagementRole(oldRole);
        
        
        
        if (needsPrivileges != hadPrivileges)
        {
            if (needsPrivileges)
            {
                await _identityService.SetRoleAsync(new Role.Admin(), request.UserId);
            } 
            else
            {
                bool isManagerElseWhere = await _clubMemberQueries.IsManagerInOtherClubs(request.UserId, request.ClubId, cancellationToken);
                if (!isManagerElseWhere)
                {
                    await _identityService.SetRoleAsync(new Role.User(), request.UserId);
                }
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}