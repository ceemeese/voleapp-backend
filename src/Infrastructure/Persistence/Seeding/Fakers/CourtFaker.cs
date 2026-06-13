using Bogus;
using Domain.Club;
using Domain.Court;
using Domain.Court.Enum;
using Domain.Reservation;

namespace Infrastructure.Persistence.Seeding.Fakers;

public static class CourtFaker
{
    public static List<Court> Generate(List<Club> clubs, int courtsPerClub = 4)
    {
        var courts = new List<Court>();
        var courtTypes = Enum.GetValues<CourtType>();

        foreach (var club in clubs)
        {
            for (var i = 1; i <= courtsPerClub; i++)
            {
                var faker = new Faker("es");
            
                var result = Court.Create(
                    clubId: club.Id,
                    name: $"Pista {i}",
                    type: faker.PickRandom(courtTypes),
                    basePrice: faker.Random.Decimal(10, 25),
                    isActive: true
                );

                if (result.IsSuccess)
                    courts.Add(result.Value);
            }
        }

        return courts;
    }
    
    
    public static void AddEvents(List<Court> courts, List<Club> clubs, List<Reservation> reservations)
    {
        var f = new Faker("es");

        foreach (var court in courts)
        {
            var club = clubs.First(c => c.Id == court.ClubId);
            var courtReservations = reservations.Where(r => r.CourtId == court.Id).ToList();

            var attempts = 0;
            var eventsAdded = 0;

            while (eventsAdded < 6 && attempts < 40)
            {
                attempts++;

                var date = DateOnly.FromDateTime(f.Date.Between(
                    DateTime.Today,
                    DateTime.Today.AddMonths(1)
                ));

                var dayOfWeek = (Domain.Club.Enum.DayOfWeek)date.DayOfWeek;
                var schedulesForDay = club.Schedules
                    .Where(s => s.DayOfWeek == dayOfWeek && !s.IsClosed)
                    .ToList();

                if (!schedulesForDay.Any()) continue;

                var schedule = f.PickRandom(schedulesForDay);
                var latestStart = schedule.ClosingTime.AddMinutes(-60);
                var totalSlots = (int)((latestStart - schedule.OpeningTime).TotalMinutes / 30);
                if (totalSlots <= 0) continue;

                var slot = f.Random.Int(0, totalSlots);
                var startTime = schedule.OpeningTime.AddMinutes(slot * 30);
                var endTime = startTime.AddMinutes(60);

                var startDateTime = date.ToDateTime(startTime);
                var endDateTime = date.ToDateTime(endTime);

                //comprobar solapamiento con reservas existentes
                var overlapsReservation = courtReservations.Any(r =>
                    r.Date == date &&
                    r.StartTime < endTime &&
                    r.EndTime > startTime
                );
                if (overlapsReservation) continue;

                
                var eventName = f.PickRandom(
                    "Torneo local",
                    "Clase grupal", 
                    "Mantenimiento",
                    "Exhibición",
                    "Liga interna"
                );

                var description = eventName switch
                {
                    "Torneo local"  => "Torneo organizado por el club, abierto a todos los socios",
                    "Clase grupal"  => "Clase dirigida por monitor profesional",
                    "Mantenimiento" => "Revisión y mantenimiento de la superficie de la pista",
                    "Exhibición"    => "Partido de exhibición con jugadores invitados",
                    "Liga interna"  => "Jornada de liga interna del club",
                    _ => null
                };
                
                //addEvent ya valida solapamiento con otros eventos internamente
                var result = court.AddEvent(
                    startTime: startDateTime,
                    endTime: endDateTime,
                    eventName: eventName,
                    description: description
                );

                if (result.IsSuccess)
                    eventsAdded++;
            }
        }
    }
}