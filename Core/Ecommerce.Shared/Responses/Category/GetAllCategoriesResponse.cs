using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Category;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a <see cref="GetAllCategoriesApiRequest"/>
	/// </summary>
	public class GetAllCategoriesResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="CategoryDto"/> entities if any exist
		/// </summary>
		public IEnumerable<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
	}
}