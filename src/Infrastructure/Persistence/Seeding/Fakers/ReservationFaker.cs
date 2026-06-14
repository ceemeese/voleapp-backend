using Bogus;
using Domain.Club;
using Domain.Common.ValueObjects;
using Domain.Court;
using Domain.Reservation;
using Domain.User;

namespace Infrastructure.Persistence.Seeding.Fakers;

public static class ReservationFaker
{
    public static List<Reservation> Generate(
        List<User> users,
        List<Club> clubs,
        List<Court> courts,
        int count = 200)
    {
        var results = new List<Reservation>();
        var f = new Faker("es");
        var attempts = 0;
        var maxAttempts = count * 5;

        while (results.Count < count && attempts < maxAttempts)
        {
            attempts++;
            
            var court = f.PickRandom(courts);
            var club = clubs.First(c => c.Id == court.ClubId);
            var user = f.PickRandom(users);
            var date = DateOnly.FromDateTime(f.Date.Between(
                DateTime.Today,
                DateTime.Today.AddMonths(1)
            ));
            
            var dayOfWeek = (Domain.Club.Enum.DayOfWeek)date.DayOfWeek;
            var schedulesForDay = club.Schedules
                .Where(s => s.DayOfWeek == dayOfWeek && !s.IsClosed)
                .ToList();
            
            //escoger schedule aleatorio
            if (!schedulesForDay.Any()) continue;
            var schedule = f.PickRandom(schedulesForDay);
            
            //cálculo de última hora para que quepa partida
            var latestStart = schedule.ClosingTime.AddMinutes(-90);
            
            //slots de 30 minutos entre apertura y maximo para que quepa partida a última hora de horario
            var totalSlots = (int)((latestStart - schedule.OpeningTime).TotalMinutes / 30);
            if (totalSlots <= 0) continue;

            //cálculo random de slot + hora de apertura para saber hora de inicio
            var slot = f.Random.Int(0, totalSlots);
            var startTime = schedule.OpeningTime.AddMinutes(slot * 30);
            var endTime = startTime.AddMinutes(90);

            var basePrice = court.BasePrice;
            var discountPercent = f.Random.Double(0, 20);
            var discountAmount = basePrice * (decimal)(discountPercent / 100);
            var totalPrice = basePrice - discountAmount;
            
            var overlapsReservation = results.Any(r =>
                r.CourtId == court.Id &&
                r.Date == date &&
                r.StartTime < endTime &&
                r.EndTime > startTime
            );
            if (overlapsReservation) continue;

            var price = new PriceBreakdown(
                BasePrice: basePrice,
                TotalPrice: totalPrice,
                DiscountAmount: discountAmount,
                AppliedDiscountPercent: discountPercent,
                DiscountReason: discountPercent > 0 ? f.PickRandom("Lluvia", "Viento", "Calor", "Frío") : null
            );

            var result = Reservation.Create(
                userId: user.Id,
                clubId: club.Id,
                courtId: court.Id,
                date: date,
                startTime: startTime,
                endTime: endTime,
                price: price,
                notes: f.Random.Bool(0.3f) ? f.PickRandom(
                    "Por favor preparad las pelotas",
                    "Venimos con niños",
                    "Necesitamos raquetas de alquiler",
                    "Reserva para cumpleaños",
                    "Somos 4 jugadores"
                ) : null
            );

            if (result.IsSuccess)
                results.Add(result.Value);
        }

        return results;
    }
}