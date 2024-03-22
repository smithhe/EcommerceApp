using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Infrastructure;

namespace Ecommerce.Persistence.Contracts
{
    /// <summary>
    /// Represents a repository for the <see cref="OrderKey"/> entity.
    /// </summary>
    public interface IOrderKeyRepository
    {
        /// <summary>
        /// Adds a <see cref="OrderKey"/> to the table.
        /// </summary>
        /// <param name="entity">The <see cref="OrderKey"/> to add</param>
        /// <returns>
        /// The ID of the newly added <see cref="OrderKey"/> if successful;
        /// -1 if the INSERT operation fails.
        /// </returns>
        Task<int> AddAsync(OrderKey entity);

        /// <summary>
        /// Deletes a row in the database based on the provided <see cref="OrderKey"/>.
        /// </summary>
        /// <param name="entity">The <see cref="OrderKey"/> to delete.</param>
        /// <returns>
        /// <c>true</c> if the DELETE is successful;
        /// <c>false</c> if the DELETE fails or the <see cref="OrderKey"/> is not found.
        /// </returns>
        Task<bool> DeleteAsync(OrderKey entity);

        /// <summary>
        /// Retrieves a <see cref="OrderKey"/> from the database with the specified Order ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the <see cref="Order"/></param>
        /// <returns>
        /// The <see cref="OrderKey"/> if found;
        /// <c>null</c> if no <see cref="OrderKey"/> with the specified Order ID is found.
        /// </returns>
        Task<OrderKey?> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Retrieves a <see cref="OrderKey"/> from the database with the specified token.
        /// </summary>
        /// <param name="token">The token generated to map back to the order id</param>
        /// <returns>
        /// The <see cref="OrderKey"/> if found;
        /// <c>null</c> if no <see cref="OrderKey"/> with the specified token is found.
        /// </returns>
        Task<OrderKey?> GetByReturnKeyAsync(string token);
    }
}