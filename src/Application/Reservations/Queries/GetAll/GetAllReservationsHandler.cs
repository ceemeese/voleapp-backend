using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetAll;

internal sealed class GetAllReservationsHandler : IRequestHandler<GetAllReservations, Result<List<ReservationResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserContext _userContext;
    
    

    public GetAllReservationsHandler(IUnitOfWork unitOfWork,  IMapper mapper, IReservationRepository reservationRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _reservationRepository = reservationRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<ReservationResponse>>> Handle(GetAllReservations request,
        CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<List<ReservationResponse>>(ReservationErrors.Forbidden);     
        }

        var reservations = await _reservationRepository.GetAllReservationsAsync(request.UserId, request.ClubId,
            request.StartDateRange, request.EndDateRange, cancellationToken);
        
        var reservationsMapped = _mapper.Map<List<ReservationResponse>>(reservations);
        return Result.Success(reservationsMapped);
    }
}
