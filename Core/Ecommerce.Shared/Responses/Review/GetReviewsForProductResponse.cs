using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get all Reviews for a Product
	/// </summary>
	public class GetReviewsForProductResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="ReviewDto"/> entities if any exist
		/// </summary>
		public IEnumerable<ReviewDto> Reviews { get; set; } = Array.Empty<ReviewDto>();
	}
}