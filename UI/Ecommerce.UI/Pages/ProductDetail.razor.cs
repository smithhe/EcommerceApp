using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.Shared.Responses.Review;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Modals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.UI.Pages
{
	public partial class ProductDetail
	{
		[Parameter] public string ProductId { get; set; } = null!;
		[CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		[CascadingParameter] public IModalService Modal { get; set; } = null!;
		
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IReviewService ReviewService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public ICartService CartService { get; set; } = null!;

		private ProductDto? Product { get; set; }

		private ReviewDto ReviewModel { get; set; } = null!;

		private const string _filledStar = "bi bi-star-fill";
		private const string _emptyStar = "bi bi-star";
		private string Star1Class { get; set; } = _emptyStar;
		private string Star2Class { get; set; } = _emptyStar;
		private string Star3Class { get; set; } = _emptyStar;
		private string Star4Class { get; set; } = _emptyStar;
		private string Star5Class { get; set; } = _emptyStar;

		private bool UserHasReview { get; set; } = false;
		private bool EditExistingReview { get; set; } = false;
		private ReviewDto? ExistingUserReview { get; set; }

		protected override async Task OnInitializedAsync()
		{
			this.ReviewModel = new ReviewDto { Stars = -1, Comments = string.Empty };

			await RefreshPageInfo();
		}

		private async Task AddToCartClick()
		{
			AuthenticationState authState = await this.AuthenticationState;

			if (authState.User.Identity?.IsAuthenticated == false)
			{
				this.NavigationManager.NavigateTo("/Login");
				return;
			}
			
			ModalParameters parameters = new ModalParameters { { nameof(AddToCartModal.SelectedProduct), this.Product! } };
			
			IModalReference formModal = this.Modal.Show<AddToCartModal>("Add Item To Cart", parameters);
			ModalResult result = await formModal.Result;

			if (result.Cancelled)
			{
				return;
			}

			CartItemDto newCartItem = (CartItemDto)result.Data!;

			CreateCartItemResponse response = await this.CartService.AddItemToCart(newCartItem);

			if (response.Success)
			{
				this.ToastService.ShowSuccess(response.Message!);
				return;
			}
			
			if (response.ValidationErrors.Any())
			{
				foreach (string validationError in response.ValidationErrors)
				{
					this.ToastService.ShowError(validationError);
				}
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
		}

		private async Task RefreshPageInfo()
		{
			AuthenticationState authState = await this.AuthenticationState;
			
			GetProductByIdResponse productResponse = await this.ProductService.GetProductById(Convert.ToInt32(this.ProductId));

			if (productResponse.Success)
			{
				this.Product = productResponse.Product;

				//Filter out the review of the logged in user from the overall list
				if (string.IsNullOrEmpty(authState.User.Identity?.Name ?? string.Empty) == false)
				{
					this.Product!.CustomerReviews = this.Product.CustomerReviews.Where(review => string.Equals(review.UserName, authState.User.Identity!.Name) == false);
				}
			}
			else
			{
				this.ToastService.ShowError(productResponse.Message!);
				return;
			}
			
			GetUserReviewForProductResponse existingReviewResponse = await this.ReviewService.GetUserReview(authState.User.Identity?.Name ?? string.Empty, Convert.ToInt32(this.ProductId));

			if (existingReviewResponse.Success && existingReviewResponse.UserReview != null)
			{
				this.UserHasReview = true;
				this.ExistingUserReview = existingReviewResponse.UserReview;
			}
		}

		private void OnStarMouseOver(int starNum)
		{
			for (int i = starNum; i > 0; i--)
			{
				switch (i)
				{
					case 1:
						this.Star1Class = _filledStar;
						break;
					case 2:
						this.Star2Class = _filledStar;
						break;
					case 3:
						this.Star3Class = _filledStar;
						break;
					case 4:
						this.Star4Class = _filledStar;
						break;
					case 5:
						this.Star5Class = _filledStar;
						break;
				}
			}
		}

		private void OnStarMouseOut()
		{
			if (this.UserHasReview && this.EditExistingReview)
			{
				for (int i = 5; i > 0; i--)
				{
					switch (i)
					{
						case 1:
							this.Star1Class = this.ExistingUserReview!.Stars < 1 ? _emptyStar : this.Star1Class;
							break;
						case 2:
							this.Star2Class = this.ExistingUserReview!.Stars < 2 ? _emptyStar : this.Star2Class;
							break;
						case 3:
							this.Star3Class = this.ExistingUserReview!.Stars < 3 ? _emptyStar : this.Star3Class;
							break;
						case 4:
							this.Star4Class = this.ExistingUserReview!.Stars < 4 ? _emptyStar : this.Star4Class;
							break;
						case 5:
							this.Star5Class = this.ExistingUserReview!.Stars < 5 ? _emptyStar : this.Star5Class;
							break;
					}
				}
			}
			else
			{
				for (int i = 5; i > 0; i--)
				{
					switch (i)
					{
						case 1:
							this.Star1Class = this.ReviewModel.Stars < 1 ? _emptyStar : this.Star1Class;
							break;
						case 2:
							this.Star2Class = this.ReviewModel.Stars < 2 ? _emptyStar : this.Star2Class;
							break;
						case 3:
							this.Star3Class = this.ReviewModel.Stars < 3 ? _emptyStar : this.Star3Class;
							break;
						case 4:
							this.Star4Class = this.ReviewModel.Stars < 4 ? _emptyStar : this.Star4Class;
							break;
						case 5:
							this.Star5Class = this.ReviewModel.Stars < 5 ? _emptyStar : this.Star5Class;
							break;
					}
				}
			}
			
		}

		private void OnStarClick(int starNum)
		{
			this.OnStarMouseOut();
			
			if (this.UserHasReview && this.EditExistingReview)
			{
				this.ExistingUserReview!.Stars = starNum;
				
				for (int i = 5; i > 0; i--)
				{
					switch (i)
					{
						case 1:
							this.Star1Class = this.ExistingUserReview!.Stars >= 1 ? _filledStar : _emptyStar;
							break;
						case 2:
							this.Star2Class = this.ExistingUserReview!.Stars >= 2 ? _filledStar : _emptyStar;
							break;
						case 3:
							this.Star3Class = this.ExistingUserReview!.Stars >= 3 ? _filledStar : _emptyStar;
							break;
						case 4:
							this.Star4Class = this.ExistingUserReview!.Stars >= 4 ? _filledStar : _emptyStar;
							break;
						case 5:
							this.Star5Class = this.ExistingUserReview!.Stars >= 5 ? _filledStar : _emptyStar;
							break;
					}
				}
			}
			else
			{
				this.ReviewModel.Stars = starNum;
				
				for (int i = 5; i > 0; i--)
				{
					switch (i)
					{
						case 1:
							this.Star1Class = this.ReviewModel.Stars >= 1 ? _filledStar : _emptyStar;
							break;
						case 2:
							this.Star2Class = this.ReviewModel.Stars >= 2 ? _filledStar : _emptyStar;
							break;
						case 3:
							this.Star3Class = this.ReviewModel.Stars >= 3 ? _filledStar : _emptyStar;
							break;
						case 4:
							this.Star4Class = this.ReviewModel.Stars >= 4 ? _filledStar : _emptyStar;
							break;
						case 5:
							this.Star5Class = this.ReviewModel.Stars >= 5 ? _filledStar : _emptyStar;
							break;
					}
				}
			}
		}
		
		private async Task SubmitReview()
		{
			if (this.ReviewModel.Stars < 1)
			{
				this.ToastService.ShowWarning("Please select a star rating to submit your review");
				return;
			}
			
			AuthenticationState authState = await this.AuthenticationState;
			this.ReviewModel.UserName = authState.User.Identity?.Name ?? string.Empty;
			this.ReviewModel.ProductId = Convert.ToInt32(this.ProductId);
			
			CreateReviewResponse response = await this.ReviewService.SubmitReview(this.ReviewModel);

			if (response.Success)
			{
				await RefreshPageInfo();
				this.ReviewModel.Comments = string.Empty;
				this.ToastService.ShowSuccess(response.Message!);
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
				if (response.ValidationErrors.Any())
				{
					foreach (string validationError in response.ValidationErrors)
					{
						this.ToastService.ShowWarning(validationError);
					}
				}
			}
		}

		private async Task DeleteReview()
		{
			DeleteReviewResponse response = await this.ReviewService.RemoveReview(this.ExistingUserReview!);

			if (response.Success)
			{
				this.ToastService.ShowSuccess(response.Message!);
				this.ExistingUserReview = null;
				this.UserHasReview = false;
				this.EditExistingReview = false;

				this.Star1Class = _emptyStar;
				this.Star2Class = _emptyStar;
				this.Star3Class = _emptyStar;
				this.Star4Class = _emptyStar;
				this.Star5Class = _emptyStar;
				
				await RefreshPageInfo();
				return;
			}
			
			this.ToastService.ShowError(response.Message!);
		}

		private void EditReviewClick()
		{
			this.EditExistingReview = true;
			this.OnStarClick(this.ExistingUserReview!.Stars);
		}

		private void CancelEditing()
		{
			this.EditExistingReview = false;
		}

		private async Task UpdateReview()
		{
			UpdateReviewResponse response = await this.ReviewService.UpdateReview(this.ExistingUserReview!);

			if (response.Success)
			{
				this.ToastService.ShowSuccess(response.Message!);
				this.EditExistingReview = false;
				
				await RefreshPageInfo();
				return;
			}
			
			this.ToastService.ShowError(response.Message!);

			if (response.ValidationErrors.Any())
			{
				foreach (string validationError in response.ValidationErrors)
				{
					this.ToastService.ShowWarning(validationError);
				}
			}
		}
		
		
	}
}