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
	/// Represents a implementation of the <see cref="IProductAsyncRepository"/> interface
	/// </summary>
	public class ProductAsyncRepository : IProductAsyncRepository
	{
		private readonly ILogger<ProductAsyncRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "Product";
		private const string _connectionStringName = "datastorage";

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public ProductAsyncRepository(ILogger<ProductAsyncRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Product"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// The <see cref="Product"/> if found;
		/// <c>null</c> if no <see cref="Product"/> with the specified ID is found.
		/// </returns>
		public async Task<Product?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			Product? product = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Product row for {id}");
				}
				
				connection.Close();
			}

			return product;
		}

		/// <summary>
		/// Adds a <see cref="Product"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="Product"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="Product"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(Product entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} " +
				"(Name, Description, Price, AverageRating, QuantityAvailable, ImageUrl, CategoryId, CreatedBy, CreatedDate) " +
				"VALUES " +
				"(@Name, @Description, @Price, @AverageRating, @QuantityAvailable, @ImageUrl, @CategoryId, @CreatedBy, @CreatedDate)" +
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
						this._logger.LogError(e, "SQL Error when adding new Product");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="Product"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Product"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(Product entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET Name = @Name,
                Description = @Description,
                Price = @Price,
                AverageRating = @AverageRating,
                QuantityAvailable = @QuantityAvailable,
                ImageUrl = @ImageUrl,
                CategoryId = @CategoryId,
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
						this._logger.LogError(e, $"SQL Error when updating Product {entity.Id}");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="Product"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Product"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="Product"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(Product entity)
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
						this._logger.LogError(e, $"SQL Error when deleting Product {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="Product"/> entities from the database with the specified <see cref="Category"/> ID.
		/// </summary>
		/// <param name="categoryId">The ID of the <see cref="Category"/> to find all corresponding <see cref="Product"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Product"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<Product>> ListAllAsync(int categoryId)
		{
			IEnumerable<Product> products = Array.Empty<Product>();
			const string sql = $"SELECT * FROM {_tableName} WHERE CategoryId = @CategoryId";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					products = await connection.QueryAsync<Product>(sql, new { CategoryId = categoryId });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching all Product rows for Category {categoryId}");
				}
				
				connection.Close();
			}

			return products;
		}

		/// <summary>
		/// Checks the table to see if the Name of a <see cref="Product"/> already exists
		/// </summary>
		/// <param name="name">The name to check for</param>
		/// <returns>
		/// <c>false</c> if found;
		/// <c>true</c> if not found
		/// </returns>
		public async Task<bool> IsNameUnique(string name)
		{
			Product? product = await GetByNameAsync(name);

			return product == null;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Product"/> from the database with the specified Name.
		/// </summary>
		/// <param name="name">The Name of the <see cref="Product"/></param>
		/// <returns>
		/// The <see cref="Product"/> if found;
		/// <c>null</c> if no <see cref="Product"/> with the specified Name is found.
		/// </returns>
		private async Task<Product?> GetByNameAsync(string name)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Name = @Name";
			Product? product = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					product = await connection.QueryFirstOrDefaultAsync<Product?>(sql, new { Name = name });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Category row with name {name}");
				}
				
				connection.Close();
			}

			return product;
		}
	}
}