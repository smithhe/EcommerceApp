using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Represents a CRUD repository for entities of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	public interface IAsyncRepository<T> where T : class
	{
		/// <summary>
		/// Retrieves a <typeparamref name="T"/> from the database with the specified ID.
		/// </summary>
		/// <param name="id">The unique identifier of the <typeparamref name="T"/></param>
		/// <returns>
		/// The <typeparamref name="T"/> if found;
		/// <c>null</c> if no <typeparamref name="T"/> with the specified ID is found.
		/// </returns>
		Task<T?> GetByIdAsync(int id);
		
		/// <summary>
		/// Adds a <typeparamref name="T"/> to the table.
		/// </summary>
		/// <param name="entity">The <typeparamref name="T"/> to add</param>
		/// <returns>
		/// The ID of the newly added <typeparamref name="T"/> if successful;
		/// -1 if the INSERT operation fails.
		/// </returns>
		Task<int> AddAsync(T entity);
		
		/// <summary>
		/// Updates a row in the database based on the provided <typeparamref name="T"/>.
		/// </summary>
		/// <param name="entity">The <typeparamref name="T"/> with updated data.</param>
		/// <returns>
		/// <c>true</c> if the UPDATE is successful;
		/// <c>false</c> if the UPDATE fails or the entity is not found.
		/// </returns>
		Task<bool> UpdateAsync(T entity);
		
		/// <summary>
		/// Deletes a row in the database based on the provided <typeparamref name="T"/>.
		/// </summary>
		/// <param name="entity">The <typeparamref name="T"/> to delete.</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or the <typeparamref name="T"/> is not found.
		/// </returns>
		Task<bool> DeleteAsync(T entity);
	}
}