namespace Ecommerce.Shared.Requests.Product
{
	/// <summary>
	/// A Api request to get all Products in a Category
	/// </summary>
	public class GetAllProductsByCategoryIdApiRequest
	{
		/// <summary>
		/// Id of the Category to find all Products for
		/// </summary>
		public int CategoryId { get; set; }
	}
}