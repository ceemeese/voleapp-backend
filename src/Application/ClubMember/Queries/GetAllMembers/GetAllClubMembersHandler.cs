using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetAllMembers;

internal sealed class GetAllClubMembersHandler : IRequestHandler<GetAllClubMembers, Result<List<ClubMemberCompleteResponse>>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;

    public GetAllClubMembersHandler(IClubRepository clubRepository, IClubMemberQueries clubMemberQueries, IUserContext userContext)
    {
        _clubRepository = clubRepository;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
    }

    public async Task<Result<List<ClubMemberCompleteResponse>>> Handle(GetAllClubMembers request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<List<ClubMemberCompleteResponse>>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<List<ClubMemberCompleteResponse>>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<List<ClubMemberCompleteResponse>>(ClubErrors.NotFound(request.ClubId));
        }
        
        var members = await _clubMemberQueries.GetMembersByClubId(request.ClubId, request.Search, cancellationToken);
        return Result.Success(members);
    }
}