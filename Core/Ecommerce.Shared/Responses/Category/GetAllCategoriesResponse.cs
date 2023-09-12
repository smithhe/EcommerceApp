using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Category
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get all Categories
	/// </summary>
	public class GetAllCategoriesResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="CategoryDto"/> entities if any exist
		/// </summary>
		public IEnumerable<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
	}
}