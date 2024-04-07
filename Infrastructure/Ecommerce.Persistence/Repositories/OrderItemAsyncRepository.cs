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
	/// Represents a implementation of the <see cref="IOrderItemAsyncRepository"/> interface
	/// </summary>
	public class OrderItemAsyncRepository : IOrderItemAsyncRepository
	{
		private readonly ILogger<OrderItemAsyncRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public OrderItemAsyncRepository(ILogger<OrderItemAsyncRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
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
			OrderItem? orderItem = null;

			try
			{
				orderItem = await this._dbContext.OrderItems.FirstOrDefaultAsync(oi => oi.Id == id);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching OrderItem row for {id}");
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
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.OrderItems.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new OrderItem");
					await transaction.RollbackAsync();
					return -1;
				}	
			}
			
			return entity.Id;
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
			
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					OrderItem? existingOrderItem = await this._dbContext.OrderItems.FirstOrDefaultAsync(oi => oi.Id == entity.Id);
					
					if (existingOrderItem == null)
					{
						return false;
					}
					
					existingOrderItem.Quantity = entity.Quantity;
					existingOrderItem.Price = entity.Price;
					existingOrderItem.ProductName = entity.ProductName;
					existingOrderItem.ProductDescription = entity.ProductDescription;
					existingOrderItem.ProductSku = entity.ProductSku;
					existingOrderItem.LastModifiedBy = entity.LastModifiedBy;
					existingOrderItem.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating OrderItem {entity.Id}");
					await transaction.RollbackAsync();
				}	
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

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.OrderItems.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting OrderItem {entity.Id}");
					await transaction.RollbackAsync();
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
			
			try
			{
				orderItems = await this._dbContext.OrderItems.Where(oi => oi.OrderId == orderId).ToArrayAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching all OrderItem rows for Order {orderId}");
			}

			return orderItems;
		}
	}
}