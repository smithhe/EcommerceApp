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
	/// Represents a implementation of the <see cref="IOrderAsyncRepository"/> interface
	/// </summary>
	public class OrderAsyncRepository : IOrderAsyncRepository
	{
		private readonly ILogger<OrderAsyncRepository> _logger;
		private readonly EcommercePersistenceDbContext _dbContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderAsyncRepository"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="dbContext">The <see cref="EcommercePersistenceDbContext"/> instance for database access</param>
		public OrderAsyncRepository(ILogger<OrderAsyncRepository> logger, EcommercePersistenceDbContext dbContext)
		{
			this._logger = logger;
			this._dbContext = dbContext;
		}
		
		/// <summary>
		/// Retrieves a <see cref="Order"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Order"/></param>
		/// <returns>
		/// The <see cref="Category"/> if found;
		/// <c>null</c> if no <see cref="Order"/> with the specified ID is found.
		/// </returns>
		public async Task<Order?> GetByIdAsync(int id)
		{
			Order? order = null;
			
			try
			{
				order = await this._dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching Order row for {id}");
			}

			return order;
		}

		/// <summary>
		/// Adds a <see cref="Order"/> to the table.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> to add</param>
		/// <returns>
		/// The ID of the newly added <see cref="Order"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		public async Task<int> AddAsync(Order entity)
		{
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					await this._dbContext.Orders.AddAsync(entity);
					await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, "SQL Error when adding new Order");
					await transaction.RollbackAsync();
				}	
			}
			
			return entity.Id;
		}

		/// <summary>
		/// Updates a row in the database based on the provided <see cref="Order"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		public async Task<bool> UpdateAsync(Order entity)
		{
			int rowsEffected = -1;
			
			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					Order? existingOrder = await this._dbContext.Orders.FirstOrDefaultAsync(o => o.Id == entity.Id);
					
					if (existingOrder == null)
					{
						return false;
					}
					
					existingOrder.Status = entity.Status;
					existingOrder.Total = entity.Total;
					existingOrder.PayPalRequestId = entity.PayPalRequestId;
					existingOrder.LastModifiedBy = entity.LastModifiedBy;
					existingOrder.LastModifiedDate = entity.LastModifiedDate;
					
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when updating Order {entity.Id}");
					await transaction.RollbackAsync();
				}	
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Deletes a row in the database based on the provided <see cref="Order"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Order"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <see cref="Order"/> is not found.
		/// </returns>
		public async Task<bool> DeleteAsync(Order entity)
		{
			int rowsEffected = -1;

			await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					this._dbContext.Orders.Remove(entity);
					rowsEffected = await this._dbContext.SaveChangesAsync();
					
					await transaction.CommitAsync();
				}
				catch (Exception e)
				{
					this._logger.LogError(e, $"SQL Error when deleting Order {entity.Id}");
					await transaction.RollbackAsync();
				}
			}

			return rowsEffected == 1;
		}

		/// <summary>
		/// Retrieves all <see cref="Order"/> entities from the database with the specified <see cref="EcommerceUser"/> ID.
		/// </summary>
		/// <param name="userId">The ID of the <see cref="EcommerceUser"/> to find all corresponding <see cref="Order"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Order"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		public async Task<IEnumerable<Order>> ListAllAsync(Guid userId)
		{
			IEnumerable<Order> orders = Array.Empty<Order>();
			
			try
			{
				orders = await this._dbContext.Orders.Where(o => o.UserId == userId).ToArrayAsync();
			}
			catch (Exception e)
			{
				this._logger.LogError(e, $"SQL Error when fetching all Orders rows for User {userId}");
			}

			return orders;
		}
	}
}