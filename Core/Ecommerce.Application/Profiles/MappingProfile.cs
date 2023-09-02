using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Application.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Category, CategoryDto>().ReverseMap();
			CreateMap<Order, OrderDto>().ReverseMap();
			CreateMap<OrderItem, OrderItemDto>().ReverseMap();
			CreateMap<Product, ProductDto>().ReverseMap();
			CreateMap<Review, ReviewDto>().ReverseMap();
		}
	}
}