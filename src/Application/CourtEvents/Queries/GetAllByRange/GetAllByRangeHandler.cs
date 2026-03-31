using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetAllByRange;

internal sealed class GetAllByRangeHandler : IRequestHandler<GetAllByRange, Result<List<CourtEventCompleteResponse>>>
{
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly ICourtEventQueries _courtEventQueries;

    public GetAllByRangeHandler(IUserContext userContext, IClubMemberQueries clubMemberQueries, ICourtEventQueries courtEventQueries)
    {
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _courtEventQueries = courtEventQueries;
    }

    public async Task<Result<List<CourtEventCompleteResponse>>> Handle(GetAllByRange request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<List<CourtEventCompleteResponse>>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure < List < CourtEventCompleteResponse>>(ClubErrors.Forbidden);
            }
        }
        
        var events = await _courtEventQueries.GetByClubIdAsync(request.ClubId, request.StartDate, request.EndDate, cancellationToken);
        return Result.Success(events);
    }
}