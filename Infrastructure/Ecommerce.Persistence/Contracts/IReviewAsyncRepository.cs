using Ecommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="T:Ecommerce.Persistence.Contracts.IAsyncRepository`1"/> interface with an additional method for <see cref="Review"/> entities
	/// </summary>
	public interface IReviewAsyncRepository : IAsyncRepository<Review>
	{
		/// <summary>
		/// Retrieves all <see cref="Review"/> entities from the database with the specified <see cref="Product"/> ID.
		/// </summary>
		/// <param name="productId">The ID of the <see cref="Product"/> to find all corresponding <see cref="Review"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Review"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// <c>null</c> if an error occurs.
		/// </returns>
		Task<IEnumerable<Review>?> ListAllAsync(int productId);

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
		Task<Review?> GetUserReviewForProduct(string userName, int productId);

		/// <summary>
		/// Calculates the average value of all star ratings for a Product
		/// </summary>
		/// <param name="productId">The unique identifier of the Product</param>
		/// <returns>
		/// Returns the average of all ratings for a product;
		/// 0 is none exist for the product
		/// </returns>
		Task<decimal> GetAverageRatingForProduct(int productId);
	}
}