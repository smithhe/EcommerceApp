using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Product
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a requests to get all Products for a Category
	/// </summary>
	public class GetAllProductsByCategoryIdResponse : BaseResponse
	{
		/// <summary>
		/// The collection of Products if any exist for in the Category
		/// </summary>
		public IEnumerable<ProductDto> Products { get; set; } = Array.Empty<ProductDto>();
	}
}