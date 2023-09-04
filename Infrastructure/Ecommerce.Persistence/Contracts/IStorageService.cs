using System.IO;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	public interface IStorageService
	{
		Task<bool> UploadFileAsync(string remoteStoragePath, string fileName, Stream file);
		Task<bool> DeleteFileAsync(string remoteStoragePath, string fileName);
	}
}