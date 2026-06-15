using Application.Abstractions.DTO.CourtEvent;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Court;
using Domain.Reservation;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Register;

internal sealed class RegisterCourtEventHandler : IRequestHandler<RegisterCourtEvent, Result<CourtEventResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly ICourtRepository _courtRepository;
    private readonly IMapper _mapper;
    private readonly IReservationRepository _reservationRepository;
    
    public RegisterCourtEventHandler(IUnitOfWork unitOfWork, IUserContext userContext,  IClubMemberQueries clubMemberQueries,  ICourtRepository courtRepository, IMapper mapper, IReservationRepository reservationRepository)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _courtRepository = courtRepository;
        _mapper = mapper;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<CourtEventResponse>> Handle(RegisterCourtEvent request, CancellationToken cancellationToken)
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
        
        var hasConflictReservations = await _reservationRepository
            .ExistsConflictAsync(court.Id, 
                DateOnly.FromDateTime(request.StartTime),
                TimeOnly.FromDateTime(request.StartTime),
                TimeOnly.FromDateTime(request.EndTime),
                cancellationToken);

        if (hasConflictReservations)
            return Result.Failure<CourtEventResponse>(ReservationErrors.TimeSlotOccupied);
        
        var courtEventResult = court.AddEvent(request.StartTime, request.EndTime, request.EventName, request.Description);
        if (courtEventResult.IsFailure)
        {
            return Result.Failure<CourtEventResponse>(courtEventResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var courtEventMapped = _mapper.Map<CourtEventResponse>(courtEventResult.Value);
        return Result.Success(courtEventMapped);
    }
}