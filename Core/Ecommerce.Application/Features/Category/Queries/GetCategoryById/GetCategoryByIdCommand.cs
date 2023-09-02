using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Queries.GetCategoryById
{
	public class GetCategoryByIdCommand : IRequest<GetCategoryByIdResponse>
	{
		public int Id { get; set; }
	}
}