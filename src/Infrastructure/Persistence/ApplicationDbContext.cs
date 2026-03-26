using Application.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Court;
using Domain.Court.Entities;
using Domain.Reservation;
using Domain.User;
using Infrastructure.Persistence.Configurations;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ClubConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ClubMemberConfiguration());
        modelBuilder.ApplyConfiguration(new CourtConfiguration());
        modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new CourtEventConfiguration());
        modelBuilder.ApplyConfiguration(new ReservationConfiguration());
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<ClubMember> ClubMembers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<CourtEvent> CourtEvents { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
}