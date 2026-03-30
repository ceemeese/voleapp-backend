using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetMemberDetail;

internal sealed class GetMemberDetailHandler : IRequestHandler<GetMemberDetail, Result<ClubMemberCompleteResponse>>
{
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    
    public GetMemberDetailHandler(IClubMemberQueries clubMemberQueries,  IUserContext userContext)
    {
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
    }

    public async Task<Result<ClubMemberCompleteResponse>> Handle(GetMemberDetail request,
        CancellationToken cancellationToken)
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
        
        var member = await _clubMemberQueries.GetMembersDetail(request.ClubId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubMemberErrors.MemberNotFound(request.UserId));
        }
        return Result.Success<ClubMemberCompleteResponse>(member);
    }
}