using Ecommerce.Shared.Responses;

namespace Ecommerce.Shared.Security.Responses
{
    /// <summary>
    /// A implementation of <see cref="BaseResponse" /> for a request to update a User's password
    /// </summary>
    public class UpdatePasswordResponse : BaseResponse
    {
        /// <summary>
        /// The updated token used to authenticate requests for the user
        /// </summary>
        public string UpdatedAccessToken { get; set; } = null!;
    }
}