using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.Roles;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Register;

internal sealed class RegisterClubMemberHandler : IRequestHandler<RegisterClubMember, Result<int>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IIdentityService _identityService;

    public RegisterClubMemberHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork,  IUserContext userContext,  IClubMemberQueries clubMemberQueries, IIdentityService identityService)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _identityService = identityService;
    }

    public async Task<Result<int>> Handle(RegisterClubMember request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<int>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<int>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<int>(ClubMemberErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<int>(ClubMemberErrors.InvalidType);
        }

        var clubMemberResult = club.AddMember(request.UserId, memberRole);
        if (clubMemberResult.IsFailure)
        {
            return Result.Failure<int>(clubMemberResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<int>(clubMemberResult.Value.Id);
    }
}