using Application.Abstractions.Interfaces;
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
        if (await _db.Users.AnyAsync())
        {
            _logger.LogInformation("La base de datos ya tiene datos. Seed omitido.");
            return;
        }

        _logger.LogInformation("Iniciando seed...");

        //clubs
        var baseEmail = _configuration["Seeding__BaseEmail"]!;
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
        var baseEmail = _configuration["Seeding__BaseEmail"]!;
        var adminPassword = _configuration["Seeding__AdminPassword"]!;
        var userPassword = _configuration["Seeding__UserPassword"]!;

        //admin por club
        for (var i = 1; i <= clubCount; i++)
        {
            await CreateUserAsync(
                username: $"admin{i}",
                email: $"{baseEmail.Split('@')[0]}+admin{i}@{baseEmail.Split('@')[1]}",
                password: adminPassword,
                role: new Role.Admin(),
                domainUsers: domainUsers
            );
        }

        //30 usuarios normales
        for (var i = 1; i <= 30; i++)
        {
            await CreateUserAsync(
                username: $"user{i}",
                email: $"{baseEmail.Split('@')[0]}+user{i}@{baseEmail.Split('@')[1]}",
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
                club.AddMember(user.Id, MemberRole.Player);
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Miembros de club asignados");
    }
}