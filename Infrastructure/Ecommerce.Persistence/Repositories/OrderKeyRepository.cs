using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Ecommerce.Persistence.Repositories
{
    /// <summary>
    /// Represents a implementation of the <see cref="IOrderKeyRepository"/> interface
    /// </summary>
    public class OrderKeyRepository : IOrderKeyRepository
    {
        private readonly ILogger<OrderKeyRepository> _logger;
        private readonly IConfiguration _configuration;
        private const string _tableName = "OrderKey";
        private const string _connectionStringName = "datastorage";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderKeyRepository"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
        public OrderKeyRepository(ILogger<OrderKeyRepository> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
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
            int newId = -1;
            const string sql =
                $"INSERT INTO {_tableName} (OrderId, OrderToken, CreatedAt) " +
                "VALUES (@OrderId, @OrderToken, @CreatedAt);" +
                "SELECT LAST_INSERT_ID();";
			
            using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        newId = await connection.QuerySingleAsync<int>(sql, entity, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e, "SQL Error when adding new OrderKey");
                        transaction.Rollback();
                    }	
                }
				
                connection.Close();
            }
			
            return newId;
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
            const string sql = $"DELETE FROM {_tableName} WHERE Id = @Id";

            using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        rowsEffected = await connection.ExecuteAsync(sql, entity, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e, $"SQL Error when deleting OrderKey {entity.Id}");
                        transaction.Rollback();
                    }
                }
                
                connection.Close();
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
            const string sql = $"SELECT * FROM {_tableName} WHERE OrderId = @OrderId";
            OrderKey? orderKey = null;
			
            using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
            {
                connection.Open();

                try
                {
                    orderKey = await connection.QueryFirstOrDefaultAsync<OrderKey>(sql, new { OrderId = orderId });
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL Error when fetching OrderKey row for {orderId}");
                }
				
                connection.Close();
            }

            return orderKey;
        }
    }
}