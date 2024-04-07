using System;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Persistence.Repositories
{
    /// <summary>
    /// Represents a implementation of the <see cref="IOrderKeyRepository"/> interface
    /// </summary>
    public class OrderKeyRepository : IOrderKeyRepository
    {
        private readonly ILogger<OrderKeyRepository> _logger;
        private readonly EcommercePersistenceDbContext _dbContext;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderKeyRepository"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
        public OrderKeyRepository(ILogger<OrderKeyRepository> logger, EcommercePersistenceDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }
        
        /// <summary>
        /// Adds a <see cref="OrderKey"/> to the table.
        /// </summary>
        /// <param name="entity">The <see cref="OrderKey"/> to add</param>
        /// <returns>
        /// The ID of the newly added <see cref="OrderKey"/> if successful;
        /// -1 if the INSERT operation fails.
        /// </returns>
        public async Task<int> AddAsync(OrderKey entity)
        {
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await this._dbContext.OrderKeys.AddAsync(entity);
                    await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "SQL Error when adding new OrderKey");
                    await transaction.RollbackAsync();
                }	
            }
			
            return entity.Id;
        }

        /// <summary>
        /// Deletes a row in the database based on the provided <see cref="OrderKey"/>.
        /// </summary>
        /// <param name="entity">The <see cref="OrderKey"/> to delete.</param>
        /// <returns>
        /// <c>true</c> if the DELETE is successful;
        /// <c>false</c> if the DELETE fails or the <see cref="OrderKey"/> is not found.
        /// </returns>
        public async Task<bool> DeleteAsync(OrderKey entity)
        {
            int rowsEffected = -1;

            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this._dbContext.OrderKeys.Remove(entity);
                    rowsEffected = await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL Error when deleting OrderKey {entity.Id}");
                    await transaction.RollbackAsync();
                }
            }

            return rowsEffected == 1;
        }

        /// <summary>
        /// Retrieves a <see cref="OrderKey"/> from the database with the specified Order ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the <see cref="Order"/></param>
        /// <returns>
        /// The <see cref="OrderKey"/> if found;
        /// <c>null</c> if no <see cref="OrderKey"/> with the specified Order ID is found.
        /// </returns>
        public async Task<OrderKey?> GetByOrderIdAsync(int orderId)
        {
            OrderKey? orderKey = null;
			
            try
            {
                orderKey = await this._dbContext.OrderKeys.FirstOrDefaultAsync(ok => ok.OrderId == orderId);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL Error when fetching OrderKey row for {orderId}");
            }

            return orderKey;
        }
        
        /// <summary>
        /// Retrieves a <see cref="OrderKey"/> from the database with the specified token.
        /// </summary>
        /// <param name="token">The token generated to map back to the order id</param>
        /// <returns>
        /// The <see cref="OrderKey"/> if found;
        /// <c>null</c> if no <see cref="OrderKey"/> with the specified token is found.
        /// </returns>
        public async Task<OrderKey?> GetByReturnKeyAsync(string token)
        {
            OrderKey? orderKey = null;
			
            try
            {
                orderKey = await this._dbContext.OrderKeys.FirstOrDefaultAsync(ok => ok.OrderToken == token);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL Error when fetching OrderKey row for {token}");
            }

            return orderKey;
        }
    }
}