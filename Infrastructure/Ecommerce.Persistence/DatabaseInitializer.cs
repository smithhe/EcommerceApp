using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Persistence
{
    public static class DatabaseInitializer
    {
        public static void MigrateDatabase(EcommercePersistenceDbContext dbContext)
        {
            dbContext.Database.Migrate();
        }

        public static void PostMigrationUpdates(EcommercePersistenceDbContext dbContext)
        {
            // Create the event to delete the OrderKey records older than 3 hours
            dbContext.Database.ExecuteSqlRaw(
                """
                    CREATE EVENT IF NOT EXISTS DeleteOrderKey
                    ON SCHEDULE EVERY 1 MINUTE
                    DO
                    DELETE FROM OrderKey WHERE CreatedAt < (NOW() - INTERVAL 3 HOUR);
                """);
        }
    }
}