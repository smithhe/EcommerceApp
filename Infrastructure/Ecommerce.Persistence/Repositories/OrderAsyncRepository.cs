using Dapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Repositories
{
	/// <summary>
	/// Represents a implementation of the <see cref="IOrderAsyncRepository"/> interface
	/// </summary>
	public class OrderAsyncRepository : IOrderAsyncRepository
	{
		private readonly ILogger<OrderAsyncRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "`Order`";
		private const string _connectionStringName = "datastorage";

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public OrderAsyncRepository(ILogger<OrderAsyncRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Order"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Order"/></param>
		/// <returns>
		/// The <see cref="Category"/> if found;
		/// <c>null</c> if no <see cref="Order"/> with the specified ID is found.
		/// </returns>
		public async Task<Order?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			Order? order = null;
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					order = await connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Order row for {id}");
				}
				
				connection.Close();
			}

			return order;
		}

		/// <summary>
		/// Adds a <see cref="Order"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="Order"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(Order entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} (UserId, Status, Total, PayPalRequestId, CreatedBy, CreatedDate) " +
				"VALUES (@UserId, @Status, @Total, @PayPalRequestId, @CreatedBy, @CreatedDate);" +
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
						this._logger.LogError(e, "SQL Error when adding new Order");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="Order"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(Order entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET UserId = @UserId,
                Status = @Status,
                Total = @Total,
                PayPalRequestId = @PayPalRequestId,
                LastModifiedBy = @LastModifiedBy,
                LastModifiedDate = @LastModifiedDate
            WHERE Id = @Id";
			
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
						this._logger.LogError(e, $"SQL Error when updating Order {entity.Id}");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="Order"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="Order"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(Order entity)
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
						this._logger.LogError(e, $"SQL Error when deleting Order {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="Order"/> entities from the database with the specified <see cref="EcommerceUser"/> ID.
		/// </summary>
		/// <param name="userId">The ID of the <see cref="EcommerceUser"/> to find all corresponding <see cref="Order"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Order"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<Order>> ListAllAsync(Guid userId)
		{
			IEnumerable<Order> orders = Array.Empty<Order>();
			const string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					orders = await connection.QueryAsync<Order>(sql, new { UserId = userId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching all Orders rows for User {userId}");
				}
				
				connection.Close();
			}

			return orders;
		}
	}
}