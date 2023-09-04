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
	/// Represents a implementation of the <see cref="IReviewAsyncRepository"/> interface
	/// </summary>
	public class ReviewAsyncRepository : IReviewAsyncRepository
	{
		private readonly ILogger<ReviewAsyncRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "Review";
		private const string _connectionStringName = "datastorage";
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReviewAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public ReviewAsyncRepository(ILogger<ReviewAsyncRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Review"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Review"/></param>
		/// <returns>
		/// The <see cref="Review"/> if found;
		/// <c>null</c> if no <see cref="Review"/> with the specified ID is found.
		/// </returns>
		public async Task<Review?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			Review? review = null;
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					review = await connection.QueryFirstOrDefaultAsync<Review>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Review row for {id}");
				}
				
				connection.Close();
			}

			return review;
		}

		/// <summary>
		/// Adds a <see cref="Review"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="Review"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="Review"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(Review entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} (ProductId, Stars, Comments, CreatedBy, CreatedDate) " +
				"VALUES (@ProductId, @Stars, Comments, @CreatedBy, @CreatedDate)" +
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
						this._logger.LogError(e, "SQL Error when adding new Review");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="Review"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Review"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(Review entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET ProductId = @ProductId,
                Stars = @Stars,
                Comments = @Comments,
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
						this._logger.LogError(e, $"SQL Error when updating Review {entity.Id}");
						transaction.Rollback();
					}
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="Review"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Review"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="Review"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(Review entity)
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
						this._logger.LogError(e, $"SQL Error when deleting Review {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="Review"/> entities from the database with the specified <see cref="Product"/> ID.
		/// </summary>
		/// <param name="productId">The ID of the <see cref="Product"/> to find all corresponding <see cref="Review"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Review"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<Review>> ListAllAsync(int productId)
		{
			IEnumerable<Review> reviews = Array.Empty<Review>();
			const string sql = $"SELECT * FROM {_tableName} WHERE ProductId = @ProductId";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					reviews = await connection.QueryAsync<Review>(sql, new { ProductId = productId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching all Review rows for Product {productId}");
				}
				
				connection.Close();
			}

			return reviews;
		}

		/// <summary>
		/// Retrieves a <see cref="Review"/> from the database with the specified UserId and ProductId
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/></param>
		/// <param name="productId">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// The <see cref="Review"/> if found;
		/// <c>null</c> if no <see cref="Review"/> with the specified UserId and ProductId is found.
		/// </returns>
		public async Task<Review?> GetUserReviewForProduct(Guid userId, int productId)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId AND ProductId = @ProductId";
			Review? review = null;
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					review = await connection.QueryFirstOrDefaultAsync<Review>(sql, new { UserId = userId, ProductId = productId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Review row for user {userId} on product {productId}");
				}
				
				connection.Close();
			}

			return review;
		}
	}
}