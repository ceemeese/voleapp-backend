using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetAllMembers;

internal sealed class GetAllClubMembersHandler : IRequestHandler<GetAllClubMembers, Result<List<ClubMemberCompleteResponse>>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetAllClubMembersHandler(IClubRepository clubRepository, IClubMemberQueries clubMemberQueries)
    {
        _clubRepository = clubRepository;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<List<ClubMemberCompleteResponse>>> Handle(GetAllClubMembers request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<List<ClubMemberCompleteResponse>>(ClubErrors.NotFound(request.ClubId));
        }
        
        var members = await _clubMemberQueries.GetMembersByClubId(request.ClubId, request.Search, cancellationToken);
        return Result.Success(members);
    }
}