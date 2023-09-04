using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class ProductDetail
	{
		[Parameter] public string ProductId { get; set; } = null!;
		
		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;

		private ProductDto? Product { get; set; }

		protected override async Task OnInitializedAsync()
		{
			GetProductByIdResponse response = await this.ProductService.GetProductById(Convert.ToInt32(this.ProductId));

			if (response.Success)
			{
				this.Product = response.Product;
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}
	}
}