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
	/// Represents a implementation of the <see cref="IReviewAsyncRepository"/> interface
	/// </summary>
	public class ReviewAsyncRepository : IReviewAsyncRepository
	{
		private readonly ILogger<ReviewAsyncRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReviewAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public ReviewAsyncRepository(ILogger<ReviewAsyncRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
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
			Review? review = null;
			
			try
			{
				review = await this._dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Review row for {id}");
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
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.Reviews.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new Review");
					await transaction.RollbackAsync();
					return -1;
				}	
			}
			
			return entity.Id;
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					Review? existingReview = await this._dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == entity.Id);
					
					if (existingReview == null)
					{
						return false;
					}
					
					existingReview.UserName = entity.UserName;
					existingReview.Comments = entity.Comments;
					existingReview.Stars = entity.Stars;
					existingReview.LastModifiedBy = entity.LastModifiedBy;
					existingReview.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating Review {entity.Id}");
					await transaction.RollbackAsync();
				}
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.Reviews.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting Review {entity.Id}");
					await transaction.RollbackAsync();
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
		/// <c>null</c> if an error occurs.
		/// </returns>
		public async Task<IEnumerable<Review>?> ListAllAsync(int productId)
		{
			IEnumerable<Review> reviews;
			
			try
			{
				reviews = await this._dbContext.Reviews.Where(r => r.ProductId == productId).ToArrayAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching all Review rows for Product {productId}");
				return null;
			}

			return reviews;
		}

		/// <summary>
		/// Retrieves a <see cref="Review"/> from the database with the specified UserId and ProductId
		/// </summary>
		/// <param name="userName">The UserName of the <see cref="EcommerceUser"/></param>
		/// <param name="productId">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// The <see cref="Review"/> if found;
		/// A new <see cref="Review"/> with an ID of -1 if no <see cref="Review"/> with the specified UserId and ProductId is found.
		/// <c>null</c> if an error occurs.
		/// </returns>
		public async Task<Review?> GetUserReviewForProduct(string userName, int productId)
		{
			Review? review;
			
			try
			{
				review = await this._dbContext.Reviews.FirstOrDefaultAsync(r => string.Equals(r.UserName, userName) && r.ProductId == productId);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Review row for user {userName} on product {productId}");
				return null;
			}
			
			if (review == null)
			{
				review = new Review { Id = -1 };
			}

			return review;
		}

		/// <summary>
		/// Calculates the average value of all star ratings for a Product
		/// </summary>
		/// <param name="productId">The unique identifier of the Product</param>
		/// <returns>
		/// Returns the average of all ratings for a product;
		/// 0 is none exist for the product
		/// </returns>
		public async Task<decimal> GetAverageRatingForProduct(int productId)
		{
			double average = 0;
			
			try
			{
				IQueryable<Review> reviews = this._dbContext.Reviews.Where(r => r.ProductId == productId);
				
				if (reviews.Any())
				{
					average = await reviews.AverageAsync(r => r.Stars);
				}
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching average star rating on product {productId}");
			}
			
			return (decimal)average;
		}
	}
}