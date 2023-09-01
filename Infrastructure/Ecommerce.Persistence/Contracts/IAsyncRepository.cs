using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	public interface IAsyncRepository<T> where T : class
	{
		Task<T> GetByIdAsync(int id);
		Task<int> AddAsync(T entity);
		Task<bool> UpdateAsync(T entity);
		Task<bool> DeleteAsync(T entity);
	}
}