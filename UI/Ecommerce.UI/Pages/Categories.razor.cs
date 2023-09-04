using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using Ecommerce.UI.Contracts;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class Categories
	{
		[Inject] public ICategoryService CategoryService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		private IEnumerable<CategoryDto>? CategoryList { get; set; }


		protected override async Task OnInitializedAsync()
		{
			GetAllCategoriesResponse response = await this.CategoryService.GetAllCategories();

			if (response.Success)
			{
				this.CategoryList = response.Categories;
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
			
		}
	}
}