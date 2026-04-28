using Application.Abstractions.DTO.Reservation;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Reservation;
using MediatR;
using SharedKernel;

namespace Application.Reservations.Queries.GetById;

internal sealed class GetReservationByIdHandler : IRequestHandler<GetReservationById, Result<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetReservationByIdHandler(IReservationRepository reservationRepository, IMapper mapper, IUserContext userContext)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<ReservationResponse>> Handle(GetReservationById request, CancellationToken cancellationToken)
    {
        if (_userContext.UserId == Guid.Empty)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.Unauthenticated);
        }

        var reservation = await _reservationRepository.GetReservationByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.NotFound(request.ReservationId));
        }
        
        var reservationMapped = _mapper.Map<ReservationResponse>(reservation);
        return Result.Success(reservationMapped);
    }
}