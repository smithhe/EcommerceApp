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
	/// Represents a implementation of the <see cref="IOrderItemAsyncRepository"/> interface
	/// </summary>
	public class OrderItemAsyncRepository : IOrderItemAsyncRepository
	{
		private readonly ILogger<OrderItemAsyncRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "OrderItem";
		private const string _connectionStringName = "datastorage";
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public OrderItemAsyncRepository(ILogger<OrderItemAsyncRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}

		/// <summary>
		/// Retrieves a <see cref="OrderItem"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="OrderItem"/></param>
		/// <returns>
		/// The <see cref="OrderItem"/> if found;
		/// <c>null</c> if no <see cref="OrderItem"/> with the specified ID is found.
		/// </returns>
		public async Task<OrderItem?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			OrderItem? orderItem = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					orderItem = await connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching OrderItem row for {id}");
				}
				
				connection.Close();
			}

			return orderItem;
		}

		/// <summary>
		/// Adds a <see cref="OrderItem"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="OrderItem"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="OrderItem"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(OrderItem entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} (ProductId, OrderId, Quantity, Price, CreatedBy, CreatedDate) " +
				"VALUES (@ProductId, @OrderId, @Quantity, @Price, @CreatedBy, @CreatedDate);" +
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
						this._logger.LogError(e, "SQL Error when adding new OrderItem");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="OrderItem"/>.
		/// </summary>
		/// <param name="entity">The <see cref="OrderItem"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(OrderItem entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET ProductId = @ProductId,
                OrderId = @OrderId,
                Quantity = @Quantity,
                Price = @Price,
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
						this._logger.LogError(e, $"SQL Error when updating OrderItem {entity.Id}");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="OrderItem"/>.
		/// </summary>
		/// <param name="entity">The <see cref="OrderItem"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="OrderItem"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(OrderItem entity)
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
						this._logger.LogError(e, $"SQL Error when deleting OrderItem {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="OrderItem"/> entities from the database with the specified <see cref="Order"/> ID.
		/// </summary>
		/// <param name="orderId">The ID of the <see cref="Order"/> to find all corresponding <see cref="OrderItem"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="OrderItem"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<OrderItem>> ListAllAsync(int orderId)
		{
			IEnumerable<OrderItem> orderItems = Array.Empty<OrderItem>();
			const string sql = $"SELECT * FROM {_tableName} WHERE OrderId = @OrderId";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					orderItems = await connection.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching all OrderItem rows for Order {orderId}");
				}
				
				connection.Close();
			}

			return orderItems;
		}
	}
}