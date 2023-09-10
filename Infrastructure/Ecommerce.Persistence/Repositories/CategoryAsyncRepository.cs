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
	/// Represents a implementation of the <see cref="ICategoryAsyncRepository"/> interface
	/// </summary>
	public class CategoryAsyncRepository : ICategoryAsyncRepository
	{
		private readonly ILogger<CategoryAsyncRepository> _logger;
		private readonly IConfiguration _configuration;
		private const string _tableName = "Category";
		private const string _connectionStringName = "datastorage";
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public CategoryAsyncRepository(ILogger<CategoryAsyncRepository> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Category"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Category"/></param>
		/// <returns>
		/// The <see cref="Category"/> if found;
		/// <c>null</c> if no <see cref="Category"/> with the specified ID is found.
		/// </returns>
		public async Task<Category?> GetByIdAsync(int id)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
			Category? category = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					category = await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Category row for {id}");
				}
				
				connection.Close();
			}

			return category;
		}

		/// <summary>
		/// Adds a <see cref="Category"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="Category"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="Category"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(Category entity)
		{
			int newId = -1;
			const string sql =
				$"INSERT INTO {_tableName} (Name, Summary, CreatedBy, CreatedDate) " +
				"VALUES (@Name, @Summary, @CreatedBy, @CreatedDate);" +
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
						this._logger.LogError(e, "SQL Error when adding new Category");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}
			
			return newId;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="Category"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Category"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(Category entity)
		{
			int rowsEffected = -1;
			const string sql = $@"
            UPDATE {_tableName}
            SET Name = @Name,
                Summary = @Summary,
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
						this._logger.LogError(e, $"SQL Error when updating Category {entity.Id}");
						transaction.Rollback();
					}	
				}
				
				connection.Close();
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="Category"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Category"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="Category"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(Category entity)
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
						this._logger.LogError(e, $"SQL Error when deleting Category {entity.Id}");
						transaction.Rollback();
					}
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="Category"/> rows from the database.
		/// </summary>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Category"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<Category>> ListAllAsync()
		{
			IEnumerable<Category> categories = Array.Empty<Category>();
			const string sql = $"SELECT * FROM {_tableName}";
			
			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					categories = await connection.QueryAsync<Category>(sql);
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when fetching all Category rows");
				}
				
				connection.Close();
			}

			return categories;
		}

		/// <summary>
		/// Checks the table to see if the Name of a <see cref="Category"/> already exists
		/// </summary>
		/// <param name="name">The name to check for</param>
		/// <returns>
		/// <c>false</c> if found;
		/// <c>true</c> if not found
		/// </returns>
		public async Task<bool> IsNameUnique(string name)
		{
			Category? category = await GetByNameAsync(name);

			return category == null;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Category"/> from the database with the specified Name.
		/// </summary>
		/// <param name="name">The Name of the <see cref="Category"/></param>
		/// <returns>
		/// The <see cref="Category"/> if found;
		/// <c>null</c> if no <see cref="Category"/> with the specified Name is found.
		/// </returns>
		private async Task<Category?> GetByNameAsync(string name)
		{
			const string sql = $"SELECT * FROM {_tableName} WHERE Name = @Name";
			Category? category = null;

			using (IDbConnection connection = new MySqlConnection(this._configuration.GetConnectionString(_connectionStringName)))
			{
				connection.Open();

				try
				{
					category = await connection.QueryFirstOrDefaultAsync<Category?>(sql, new { Name = name });
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when fetching Category row with name {name}");
				}
				
				connection.Close();
			}

			return category;
		}
	}
}