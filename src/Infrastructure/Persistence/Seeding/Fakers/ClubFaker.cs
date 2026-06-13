using Bogus;
using Domain.Club;
using Domain.Common.ValueObjects;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Infrastructure.Persistence.Seeding.Fakers;

public static class ClubFaker
{
    public static List<Club> Generate(string baseEmail)
    {
        var parts = baseEmail.Split('@');
        
        var clubs = new List<(string Name, string Street, string City, string PostalCode, string Country)>
        {
            ("Club Pádel Zaragoza", "Calle de San Juan Bautista de la Salle, 1", "Zaragoza", "50012", "ES"),
            ("Padel Indoor Aragon", "Camino la Raya", "Zaragoza", "50002", "ES"),
            ("Urban Sport", "Calle de Augusto Bebel, 16", "Zaragoza", "50015", "ES"),
            ("Regal Padel Club Zaragoza", "CC Alcampo Los Enlaces, N-II KM 315, 2 Local", "Zaragoza", "50012", "ES"),
            ("Padel House Zaragoza", "Calle Lituania, 78", "Utebo", "50180", "ES"),
            ("Padel Plaza", "Avenida Diagonal Plaza, 3 Nave C", "Zaragoza", "50197", "ES"),
            ("CDM Montecanal", "Calle de la Mesta s/n", "Zaragoza", "50012", "ES"),
            ("Stadium Casablanca", "Via Ibérica, 69", "Zaragoza", "50012", "ES"),
            ("Real Zaragoza CLub Tenis", "Carretera del Aeropuerto km 4", "Zaragoza", "50190", "ES"),
            ("CDM Delicias", "Calle Moreno Alcañiz, 2", "Zaragoza", "50017", "ES"),
            ("David Lloyd Zaragoza", "Calle María Montessori, 13", "Zaragoza", "50018", "ES"),
            ("CDM Almozara", "Av. de la Almozara, 65", "Zaragoza", "50003", "ES"),
            ("Stadium Venecia", "Calle Fray Julián Garcés, 100", "Zaragoza", "50007", "ES"),
            ("Padel Indoor Lleida", "Calle Alcarrás, 23", "Lleida", "25190", "ES"),
            ("Sicoris Club", "Calle Sicoris, 45", "Lleida", "25001", "ES"),
            ("Club Tenis Lleida", "Partida de Boxadors, 60", "Lleida", "25198", "ES"),
            ("Club Tennis Urgell", "Partida de Balafia, 71", "Lleida", "25196", "ES"),
            ("Padel Alcarrás", "Partida de les Sorts, 204", "Alcarrás", "25180", "ES"),
            ("Indoor Huesca", "Calle Siderurgia, 1 Nave 68", "Huesca", "22006", "ES"),
            ("X3 Padel Indoor", "Calle Alcañiz, 9", "Huesca", "22004", "ES"),
            ("Zonepadel", "Polígono Sepes, Ronda de la indústria 133", "Huesca", "22006", "ES"),
        };

        return clubs
            .Select(c =>
            {
                var f = new Faker("es");

                var clubEmail = $"{parts[0]}+club{f.Random.Number(1000, 99999)}@{parts[1]}";
                
                var addressResult = Address.Create(c.Street, c.City, c.PostalCode, c.Country);
                if (addressResult.IsFailure) return null;

                var club = Club.Create(
                    name: c.Name,
                    cif: f.Random.Replace("?########").ToUpper(),
                    address: addressResult.Value,
                    phoneNumber: f.Phone.PhoneNumber("9########"),
                    email: clubEmail
                );

                foreach (var day in Enum.GetValues<DayOfWeek>())
                {
                    if (day is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    {
                        club.AddSchedule(day, new TimeOnly(9, 0), new TimeOnly(21, 0));
                    }
                    else
                    {
                        club.AddSchedule(day, new TimeOnly(8, 0),  new TimeOnly(14, 0));
                        club.AddSchedule(day, new TimeOnly(16, 0), new TimeOnly(22, 0));
                    }
                }

                club.UpdatePricing(
                    rain:  f.Random.Decimal(5, 20),
                    windT: f.Random.Double(15, 50),
                    windD: f.Random.Decimal(10, 50),
                    heatT: f.Random.Double(35, 42),
                    heatD: f.Random.Decimal(5, 20),
                    coldT: f.Random.Double(0, 5),
                    coldD: f.Random.Decimal(5, 20)
                );

                return club;
            })
            .OfType<Club>()
            .ToList();
    }
}