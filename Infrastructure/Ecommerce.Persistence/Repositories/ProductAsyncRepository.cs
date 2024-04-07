using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Persistence.Repositories
{
	/// <summary>
	/// Represents a implementation of the <see cref="IProductAsyncRepository"/> interface
	/// </summary>
	public class ProductAsyncRepository : IProductAsyncRepository
	{
		private readonly ILogger<ProductAsyncRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public ProductAsyncRepository(ILogger<ProductAsyncRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
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
			Product? product = null;

			try
			{
				product = await this._dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Product row for {id}");
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
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.Products.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new Product");
					await transaction.RollbackAsync();
				}	
			}
			
			return entity.Id;
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					Product? existingProduct = await this._dbContext.Products.FirstOrDefaultAsync(p => p.Id == entity.Id);

					if (existingProduct == null)
					{
						return false;
					}
					
					existingProduct.Name = entity.Name;
					existingProduct.Price = entity.Price;
					existingProduct.QuantityAvailable = entity.QuantityAvailable;
					existingProduct.Description = entity.Description;
					existingProduct.AverageRating = entity.AverageRating;
					existingProduct.ImageUrl = entity.ImageUrl;
					existingProduct.LastModifiedBy = entity.LastModifiedBy;
					existingProduct.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating Product {entity.Id}");
					await transaction.RollbackAsync();
				}	
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting Product {entity.Id}");
					await transaction.RollbackAsync();
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
			
			try
			{
				products = await this._dbContext.Products.Where(p => p.CategoryId == categoryId).ToArrayAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching all Product rows for Category {categoryId}");
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
			Product? product = await this.GetByNameAsync(name);

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
			Product? product = null;

			try
			{
				product = await this._dbContext.Products.FirstOrDefaultAsync(p => string.Equals(p.Name, name));
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Category row with name {name}");
			}

			return product;
		}
	}
}