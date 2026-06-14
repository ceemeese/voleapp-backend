using System.Globalization;
using System.Text;
using Application.Abstractions.Interfaces;
using Bogus;
using Domain.Club;
using Domain.Club.Enum;
using Domain.Roles;
using Infrastructure.Persistence.Seeding.Fakers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext db,
        IIdentityService identityService,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _identityService = identityService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _db.Clubs.AnyAsync())
        {
            _logger.LogInformation("La base de datos ya tiene datos. Seed omitido");
            return;
        }

        _logger.LogInformation("Iniciando seed...");

        //clubs
        var baseEmail = _configuration.GetSection("Seeding").GetValue<string>("BaseEmail");
        
        if (string.IsNullOrEmpty(baseEmail))
        {
            _logger.LogError("Seeding__BaseEmail no está configurado en las variables de entorno");
            return;
        }

        var clubs = ClubFaker.Generate(baseEmail);
        _db.Clubs.AddRange(clubs);
        await _db.SaveChangesAsync();
        _logger.LogInformation("{Count} clubs creados.", clubs.Count);

        //usuarios
        var domainUsers = await SeedUsersAsync(clubs.Count);

        //pistas
        var courts = CourtFaker.Generate(clubs);
        _db.Courts.AddRange(courts);
        await _db.SaveChangesAsync();
        _logger.LogInformation("{Count} pistas creadas.", courts.Count);

        //clubmembers
        await SeedClubMembersAsync(domainUsers, clubs);

        //reservas
        var reservations = ReservationFaker.Generate(domainUsers, clubs, courts, count: 200);
        _db.Reservations.AddRange(reservations);
        await _db.SaveChangesAsync();
        _logger.LogInformation("{Count} reservas creadas.", reservations.Count);

        //eventos
        CourtFaker.AddEvents(courts, clubs, reservations);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Eventos de pista creados.");

        _logger.LogInformation("Seed completado.");
    }

    private async Task<List<Domain.User.User>> SeedUsersAsync(int clubCount)
    {
        var domainUsers = new List<Domain.User.User>();
        var baseEmail = _configuration.GetSection("Seeding").GetValue<string>("BaseEmail");
        var adminPassword = _configuration.GetSection("Seeding").GetValue<string>("AdminPassword");
        var userPassword = _configuration.GetSection("Seeding").GetValue<string>("UserPassword");

        var f = new Faker("es");
        
        //admin por club
        for (var i = 1; i <= clubCount; i++)
        {
            var firstName = f.Name.FirstName();
            var lastName = f.Name.LastName();
            var username = GenerateUsername(firstName, lastName, f.Random.Number(10, 99));
            var email = $"{baseEmail.Split('@')[0]}+{username}@{baseEmail.Split('@')[1]}";
            
            await CreateUserAsync(
                firstName: firstName,
                lastName: lastName,
                username: username,
                email: email,
                password: adminPassword,
                role: new Role.Admin(),
                domainUsers: domainUsers
            );
        }

        //30 usuarios normales
        for (var i = 1; i <= 30; i++)
        {
            var firstName = f.Name.FirstName();
            var lastName = f.Name.LastName();
            var username = GenerateUsername(firstName, lastName, f.Random.Number(10, 99));
            var email = $"{baseEmail.Split('@')[0]}+{username}@{baseEmail.Split('@')[1]}";
            
            await CreateUserAsync(
                firstName: firstName,
                lastName: lastName,
                username: username,
                email: $"{baseEmail.Split('@')[0]}+{username}@{baseEmail.Split('@')[1]}",
                password: userPassword,
                role: new Role.User(),
                domainUsers: domainUsers
            );
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("{Count} usuarios creados", domainUsers.Count);
        return domainUsers;
    }

    private async Task CreateUserAsync(
        string firstName,
        string lastName,
        string username,
        string email,
        string password,
        IRole role,
        List<Domain.User.User> domainUsers)
    {
        //identity
        var identityResult = await _identityService.CreateUserAsync(username, email, password);
        if (identityResult.IsFailure)
        {
            _logger.LogWarning("No se pudo crear el usuario {Email}: {Error}", email, identityResult.Error);
            return;
        }

        //rol
        await _identityService.SetRoleAsync(role, identityResult.Value);

        //entidad de dominio
        var domainUser = UserFaker.Generate(
            identityId: identityResult.Value, 
            firstName: firstName,
            lastName: lastName,
            username: username,
            email: email
        );

        _db.Users.Add(domainUser);
        domainUsers.Add(domainUser);
    }

    private async Task SeedClubMembersAsync(List<Domain.User.User> users, List<Club> clubs)
    {
        var f = new Bogus.Faker("es");
        var admins = users.Take(clubs.Count).ToList();
        var regularUsers = users.Skip(clubs.Count).ToList();

        foreach (var club in clubs)
        {
            var adminUser = admins[clubs.IndexOf(club)];
            club.AddMember(adminUser.Id, MemberRole.Admin);

            var members = regularUsers
                .OrderBy(_ => Guid.NewGuid())
                .Take(f.Random.Int(5, 15));

            foreach (var user in members)
            {
                var memberResult = club.AddMember(user.Id, MemberRole.Player);
                if (memberResult.IsFailure) continue;
                
                if (f.Random.Bool(0.7f))
                {
                    club.UpdateMember(
                        userId: user.Id,
                        role: MemberRole.Player,
                        isMember: true,
                        membershipNumber: f.Random.Replace("SOC-#####")
                    );
                }
                
                if (f.Random.Bool(0.3f))
                    club.ToggleMemberFavourite(user.Id);
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Miembros de club asignados");
    }
    
    
    
    private static string GenerateUsername(string firstName, string lastName, int number)
    {
        var raw = $"{firstName[0]}{lastName}{number}".ToLower();
        return new string(
            raw.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray()
        );
    }
}