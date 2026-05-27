using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Reservation.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reservations;

internal sealed class ReservationCreatedDomainEventHandler : INotificationHandler<ReservationCreatedDomainEvent>
{
    private readonly ILogger<ReservationCreatedDomainEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IReservationQueries _reservationQueries;

    public ReservationCreatedDomainEventHandler(ILogger<ReservationCreatedDomainEventHandler> logger,  IEmailService emailService, IReservationQueries reservationQueries)
    {
        _logger = logger;
        _emailService = emailService;
        _reservationQueries = reservationQueries;
    }
    
    
    public async Task Handle(ReservationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var reservationDto = await _reservationQueries.GetReservationCompleteByIdAsync(notification.Reservation.Id, cancellationToken);
        if (reservationDto is null)
        {
            _logger.LogWarning($"No se encuentra reserva {notification.Reservation.Id} para enviar email");
            return;
        }
        
        _logger.LogInformation(
            "Procesando envío de email para la reserva #{ReservationId} " +
            "Usuario: {PlayerName} ({Email}), Club: {ClubName}, Pista: {CourtName}, Horario: {Date} de {StartTime} a {EndTime}.",
            notification.Reservation.Id,
            reservationDto.Username,
            reservationDto.Email,
            reservationDto.ClubName,
            reservationDto.CourtName,
            reservationDto.Date.ToString("dd/MM/yyyy"),
            reservationDto.StartTime,
            reservationDto.EndTime);
        
        await _emailService.SendReservationEmailAsync(
            reservationDto.Email,
            reservationDto.Username,
            reservationDto.Date.ToString("dd/MM/yyyy"),
            $"{reservationDto.StartTime} - {reservationDto.EndTime}",
            reservationDto.CourtName,
            reservationDto.ClubName);
    }
}