using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class Products
	{
		[Parameter] public string CategoryId { get; set; } = null!;

		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		private IEnumerable<ProductDto>? ProductList { get; set; }

		protected override async Task OnInitializedAsync()
		{
			GetAllProductsByCategoryIdResponse response = await this.ProductService.GetAllProducts(Convert.ToInt32(this.CategoryId));
			
			if (response.Success)
			{
				this.ProductList = response.Products;
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}
	}
}