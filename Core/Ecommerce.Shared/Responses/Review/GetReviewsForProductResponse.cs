using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Review;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Review
{
	/// <summary>
	/// A implementation of the <see cref="BaseResponse" /> for a <see cref="GetReviewsForProductApiRequest"/>
	/// </summary>
	public class GetReviewsForProductResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="ReviewDto"/> entities if any exist
		/// </summary>
		public IEnumerable<ReviewDto> Reviews { get; set; } = Array.Empty<ReviewDto>();
	}
}