using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Court;
using Domain.Reservation;
using Domain.Reservation.Services;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Commands.Register;

internal sealed class RegisterReservationHandler : IRequestHandler<RegisterReservation, Result<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IClubRepository _clubRepository;
    private readonly IReservationService _reservationService;
    private readonly ICourtRepository _courtRepository;

    public RegisterReservationHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork, IMapper mapper,  IUserContext userContext,  IClubRepository clubRepository,  IReservationService reservationService, ICourtRepository courtRepository)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _clubRepository = clubRepository;
        _reservationService = reservationService;
        _courtRepository = courtRepository;
    }

    public async Task<Result<ReservationResponse>> Handle(RegisterReservation request,
        CancellationToken cancellationToken)
    {
        if (_userContext.UserId == Guid.Empty)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.Unauthenticated);
        }
        
        var court = await _courtRepository.GetCourtByIdAsync(request.CourtId, cancellationToken);
        if (court is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.CourtNotFound);
        }

        var reservationResult = _reservationService.BookCourt(
            _userContext.UserId, 
            court, 
            request.Date,
            request.StartTime, 
            request.EndTime, 
            request.Notes
        );

        if (reservationResult.IsFailure)
        {
            return Result.Failure<ReservationResponse>(reservationResult.Error);
        }
        
        _reservationRepository.Add(reservationResult.Value);
        
        var club = await _clubRepository.GetClubWithMembersAsync(court.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.ClubNotFound(court.ClubId));
        }
        
        var ensureMemberResult = club.EnsureMembership(_userContext.UserId);
        if (ensureMemberResult.IsFailure)
        {
            return Result.Failure<ReservationResponse>(ensureMemberResult.Error);
        }
        
       await _unitOfWork.SaveChangesAsync(cancellationToken); 
       
       var reservationMapped = _mapper.Map<ReservationResponse>(reservationResult.Value);
       return Result.Success(reservationMapped);
    }
    
}