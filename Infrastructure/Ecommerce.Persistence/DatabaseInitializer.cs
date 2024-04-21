using System;
using Ecommerce.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Persistence
{
    public static class DatabaseInitializer
    {
        public static void MigrateDatabase(EcommercePersistenceDbContext dbContext)
        {
            dbContext.Database.Migrate();
        }

        public static void PostMigrationUpdates(EcommercePersistenceDbContext dbContext, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Create the event to delete the OrderKey records older than 3 hours
            dbContext.Database.ExecuteSqlRaw(
                """
                    CREATE EVENT IF NOT EXISTS DeleteOrderKey
                    ON SCHEDULE EVERY 1 MINUTE
                    DO
                    DELETE FROM OrderKey WHERE CreatedAt < (NOW() - INTERVAL 3 HOUR);
                """);
            
            
            // Add roles to the database
            if (roleManager.RoleExistsAsync(RoleNames._admin).Result == false)
            {
                IdentityRole<Guid> role = new IdentityRole<Guid>
                {
                    Name = RoleNames._admin
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (roleResult.Succeeded == false)
                {
                    Console.WriteLine("Failed to create the admin role.");
                }
            }

            if (roleManager.RoleExistsAsync(RoleNames._user).Result == false)
            {
                IdentityRole<Guid> role = new IdentityRole<Guid>
                {
                    Name = RoleNames._user
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                
                if (roleResult.Succeeded == false)
                {
                    Console.WriteLine("Failed to create the user role.");
                }
            }
        }
    }
}