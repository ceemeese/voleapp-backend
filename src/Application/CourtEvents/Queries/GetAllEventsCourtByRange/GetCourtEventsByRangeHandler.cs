using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Queries.GetAllEventsCourtByRange;

internal sealed class GetCourtEventsByRangeHandler : IRequestHandler<GetCourtEventsByRange, Result<List<CourtEventResponse>>>
{
    private readonly IMapper _mapper;
    private readonly ICourtRepository _courtRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetCourtEventsByRangeHandler(IMapper mapper, ICourtRepository courtRepository, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _mapper = mapper;
        _courtRepository = courtRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<List<CourtEventResponse>>> Handle(GetCourtEventsByRange request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<List<CourtEventResponse>>(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtWithEventsByDateRangeAsync(request.CourtId,request.StartDate, cancellationToken, request.EndDate);
        if (court is null)
        {
            return Result.Failure<List<CourtEventResponse>>(CourtErrors.NotFound(request.CourtId));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<List<CourtEventResponse>>(ClubErrors.Forbidden);
            }
        }
        
        var eventsMapped = _mapper.Map<List<CourtEventResponse>>(court.CourtEvents);
        return Result.Success<List<CourtEventResponse>>(eventsMapped);
    }
}