using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Update;

internal sealed class UpdateCourtEventHandler : IRequestHandler<UpdateCourtEvent, Result<CourtEventResponse>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UpdateCourtEventHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork,
        IClubMemberQueries clubMemberQueries, IUserContext userContext, IMapper mapper)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _clubMemberQueries = clubMemberQueries;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<CourtEventResponse>> Handle(UpdateCourtEvent request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<CourtEventResponse>(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtWithEventsByDateRangeAsync(request.CourtId,request.StartTime, cancellationToken);
        if (court is null)
        {
            return Result.Failure<CourtEventResponse>(CourtErrors.NotFound(request.CourtId));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<CourtEventResponse>(ClubErrors.Forbidden);
            }
        }
        
        var courtEventResult = court.UpdateEvent(request.EventId, request.StartTime, request.EndTime, request.EventName, request.Description);
        if (courtEventResult.IsFailure)
        {
            return Result.Failure<CourtEventResponse>(courtEventResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var courtEventMapped = _mapper.Map<CourtEventResponse>(courtEventResult.Value);
        return Result.Success(courtEventMapped);
    }
}