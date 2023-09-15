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
	/// Represents a implementation of the <see cref="ICartItemRepository"/> interface
	/// </summary>
	public class CartItemRepository : ICartItemRepository
	{
		private readonly ILogger<CartItemRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "CartItem";
		private const string _connectionStringName = "datastorage";
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ProductAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public CartItemRepository(ILogger<CartItemRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Retrieves a <see cref="CartItem"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="CartItem"/></param>
		/// <returns>
		/// The <see cref="CartItem"/> if found;
		/// <c>null</c> if no <see cref="CartItem"/> with the specified ID is found.
		/// </returns>
		public async Task<CartItem?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			CartItem? cartItem = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					cartItem = await connection.QueryFirstOrDefaultAsync<CartItem>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching CartItem row for {id}");
				}
				
				connection.Close();
			}

			return cartItem;
		}

		/// <summary>
		/// Adds a <see cref="CartItem"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="CartItem"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="CartItem"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(CartItem entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} " +
				"(ProductId, UserId, Quantity, CreatedBy, CreatedDate) " +
				"VALUES " +
				"(@ProductId, @UserId, @Quantity, @CreatedBy, @CreatedDate);" +
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
						this._logger.LogError(e, "SQL Error when adding new CartItem");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="CartItem"/>.
		/// </summary>
		/// <param name="entity">The <see cref="CartItem"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(CartItem entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET ProductId = @ProductId,
                UserId = @UserId,
                Quantity = @Quantity,
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
						this._logger.LogError(e, $"SQL Error when updating CartItem {entity.Id}");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="CartItem"/>.
		/// </summary>
		/// <param name="entity">The <see cref="CartItem"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="CartItem"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(CartItem entity)
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
						this._logger.LogError(e, $"SQL Error when deleting CartItem {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="CartItem"/> rows from the database for a <see cref="EcommerceUser"/>.
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/> to find the cart items for</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="CartItem"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<CartItem>> ListAllAsync(Guid userId)
		{
			IEnumerable<CartItem> cartItems = Array.Empty<CartItem>();
			const string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					cartItems = await connection.QueryAsync<CartItem>(sql, new { UserId = userId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching all CartItem rows for User {userId}");
				}
				
				connection.Close();
			}

			return cartItems;
		}

		/// <summary>
		/// Removes all <see cref="CartItem"/> entities from the database for a <see cref="EcommerceUser"/>.
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/> to remove all cart items for</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or no <see cref="CartItem"/> entities are not found.
		/// </returns>
		public async Task<bool> RemoveUserCartItems(Guid userId)
		{
			int rowsEffected = -1;
			const string sql = $"DELETE FROM {_tableName} WHERE UserId = @UserId";

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				using (IDbTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						rowsEffected = await connection.ExecuteAsync(sql, new { UserId = userId }, transaction: transaction);
						transaction.Commit();
					}
					catch (Exception e)
					{
						this._logger.LogError(e, $"SQL Error when deleting CartItems for User {userId}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected >= 1;
		}

		/// <summary>
		/// Checks to see if the user already has a <see cref="CartItem"/> for the <see cref="Product"/>
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/></param>
		/// <param name="productId">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// <c>true</c> if the <see cref="CartItem"/> exists;
		/// <c>false</c> no <see cref="CartItem"/> is found.
		/// </returns>
		public async Task<bool> CartItemExistsForUser(Guid userId, int productId)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId AND ProductId = @ProductId";
			CartItem? cartItem = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					cartItem = await connection.QueryFirstOrDefaultAsync<CartItem>(sql, new { UserId = userId, ProductId = productId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when checking if a CartItem exists");
				}
				
				connection.Close();
			}

			return cartItem != null;
		}
	}
}