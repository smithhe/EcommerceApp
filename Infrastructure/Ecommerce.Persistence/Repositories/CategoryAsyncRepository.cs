using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Persistence.Repositories
{
	/// <summary>
	/// Represents a implementation of the <see cref="ICategoryAsyncRepository"/> interface
	/// </summary>
	public class CategoryAsyncRepository : ICategoryAsyncRepository
	{
		private readonly ILogger<CategoryAsyncRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public CategoryAsyncRepository(ILogger<CategoryAsyncRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
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
			Category? category = null;

			try
			{
				category = await this._dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Category row for {id}");
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
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.Categories.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new Category");
					await transaction.RollbackAsync();
				}	
			}
			
			return entity.Id;
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					Category? existingCategory = await this._dbContext.Categories.FirstOrDefaultAsync(c => c.Id == entity.Id);
					
					if (existingCategory == null)
					{
						return false;
					}
					
					existingCategory.Name = entity.Name;
					existingCategory.Summary = entity.Summary;
					existingCategory.LastModifiedBy = entity.LastModifiedBy;
					existingCategory.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating Category {entity.Id}");
					await transaction.RollbackAsync();
				}	
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.Categories.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting Category {entity.Id}");
					await transaction.RollbackAsync();
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
			
			try
			{
				categories = await this._dbContext.Categories.ToListAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "SQL Error when fetching all Category rows");
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
			Category? category = await this.GetByNameAsync(name);

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
			Category? category = null;

			try
			{
				category = await this._dbContext.Categories.FirstOrDefaultAsync(c => string.Equals(c.Name, name));
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Category row with name {name}");
			}

			return category;
		}
	}
}