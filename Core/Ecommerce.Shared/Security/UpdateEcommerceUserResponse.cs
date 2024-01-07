using Ecommerce.Shared.Responses;

namespace Ecommerce.Shared.Security
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to update a EcommerceUser
	/// </summary>
	public class UpdateEcommerceUserResponse : BaseResponse
	{
		/// <summary>
		/// The updated token used to authenticate requests for the user
		/// </summary>
		public string UpdatedAccessToken { get; set; } = null!;
	}
}