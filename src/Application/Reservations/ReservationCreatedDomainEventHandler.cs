using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Reservation.Events;
using MediatR;

namespace Application.Reservations;

internal sealed class ReservationCreatedDomainEventHandler : INotificationHandler<ReservationCreatedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCreatedDomainEventHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    
    public async Task Handle(ReservationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}