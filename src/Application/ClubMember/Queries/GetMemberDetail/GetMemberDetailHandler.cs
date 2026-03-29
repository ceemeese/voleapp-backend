using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetMemberDetail;

internal sealed class GetMemberDetailHandler : IRequestHandler<GetMemberDetail, Result<ClubMemberCompleteResponse>>
{
    private readonly IClubMemberQueries _clubMemberQueries;
    
    public GetMemberDetailHandler(IClubMemberQueries clubMemberQueries)
    {
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<ClubMemberCompleteResponse>> Handle(GetMemberDetail request,
        CancellationToken cancellationToken)
    {
        var member = await _clubMemberQueries.GetMembersDetail(request.ClubId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubMemberErrors.MemberNotFound(request.UserId));
        }
        return Result.Success<ClubMemberCompleteResponse>(member);
    }
}