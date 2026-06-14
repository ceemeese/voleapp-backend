using Infrastructure.Identity;
using Infrastructure.Persistence;
 using Microsoft.EntityFrameworkCore;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Hosting;
 
 namespace Infrastructure.Extensions;
 
 public static class MigrationExtension
 {
     public static void ApplyMigrations(this IHost app)
     {
         var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
         using (var scope = scopeFactory?.CreateScope())
         {
             var dbContext = scope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
             dbContext?.Database.Migrate();
             
             var authDb = scope?.ServiceProvider.GetRequiredService<AuthDbContext>();
             authDb?.Database.Migrate();
         }
     }
 }