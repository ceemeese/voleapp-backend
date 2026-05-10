using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.Roles;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Update;

internal sealed class UpdateClubMemberHandler : IRequestHandler<UpdateClubMember, Result<ClubMemberCompleteResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public UpdateClubMemberHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository,  IUserContext userContext, IClubMemberQueries clubMemberQueries,  IIdentityService identityService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Result<ClubMemberCompleteResponse>> Handle(UpdateClubMember request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<ClubMemberCompleteResponse>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<ClubMemberCompleteResponse>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubWithMembersAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubMemberErrors.InvalidType);
        }
        
        var currentMember = club.Members.FirstOrDefault(m => m.UserId == request.UserId);
        var oldRole = currentMember?.Role;

        var memberResult = club.UpdateMember(request.UserId, memberRole, request.IsMember, request.MembershipNumber);
        if (memberResult.IsFailure)
        {
            return Result.Failure<ClubMemberCompleteResponse>(memberResult.Error);
        }

        bool IsManagementRole(MemberRole? role) => role == MemberRole.Admin || role == MemberRole.Owner || role == MemberRole.Coach;
        
        var needsPrivileges = IsManagementRole(memberRole);
        var hadPrivileges = IsManagementRole(oldRole);
        
        if (needsPrivileges != hadPrivileges)
        {
            if (needsPrivileges)
            {
                await _identityService.SetRoleAsync(new Role.Admin(), request.UserId);
            } 
            else
            {
                var isManagerElseWhere = await _clubMemberQueries.IsManagerInOtherClubs(request.UserId, request.ClubId, cancellationToken);
                if (!isManagerElseWhere)
                {
                    await _identityService.SetRoleAsync(new Role.User(), request.UserId);
                }
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var clubMemberMapped = _mapper.Map<ClubMemberCompleteResponse>(memberResult.Value);
        return Result.Success(clubMemberMapped);
    }
}