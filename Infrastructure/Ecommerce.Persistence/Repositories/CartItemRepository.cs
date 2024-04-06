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
	/// Represents a implementation of the <see cref="ICartItemRepository"/> interface
	/// </summary>
	public class CartItemRepository : ICartItemRepository
	{
		private readonly ILogger<CartItemRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public CartItemRepository(ILogger<CartItemRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
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
			CartItem? cartItem = null;
			
			try
			{
				cartItem = await this._dbContext.CartItems.FirstOrDefaultAsync(c => c.Id == id);
					
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching CartItem row for {id}");
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
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.CartItems.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new CartItem");
					await transaction.RollbackAsync();
				}	
			}
			
			return entity.Id;
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
			
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					CartItem? existingCartItem = await this._dbContext.CartItems.FirstOrDefaultAsync(c => c.Id == entity.Id);
					
					if (existingCartItem == null)
					{
						return false;
					}
					
					existingCartItem.Quantity = entity.Quantity;
					existingCartItem.LastModifiedBy = entity.LastModifiedBy;
					existingCartItem.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating CartItem {entity.Id}");
					await transaction.RollbackAsync();
				}	
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.CartItems.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting CartItem {entity.Id}");
					await transaction.RollbackAsync();
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
			
			try
			{
				cartItems = await this._dbContext.CartItems.Where(c => c.UserId == userId).ToArrayAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching all CartItem rows for User {userId}");
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					CartItem[] cartItemsToDelete = await this._dbContext.CartItems.Where(c => c.UserId == userId).ToArrayAsync();
					this._dbContext.CartItems.RemoveRange(cartItemsToDelete);
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting CartItems for User {userId}");
					await transaction.RollbackAsync();
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
			CartItem? cartItem = null;

			try
			{
				cartItem = await this._dbContext.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when checking if a CartItem exists");
			}

			return cartItem != null;
		}
	}
}