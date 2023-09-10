using Blazored.Toast.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.Shared.Responses.Review;
using Ecommerce.UI.Contracts;
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
		
		[Inject] public IProductService ProductService { get; set; } = null!;
		[Inject] public IReviewService ReviewService { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;

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
			AuthenticationState authState = await this.AuthenticationState;
			
			GetProductByIdResponse response = await this.ProductService.GetProductById(Convert.ToInt32(this.ProductId));

			if (response.Success)
			{
				this.Product = response.Product;

				//Filter out the review of the logged in user from the overall list
				if (string.IsNullOrEmpty(authState.User.Identity?.Name ?? string.Empty) == false)
				{
					this.Product!.CustomerReviews = this.Product.CustomerReviews.Where(review => string.Equals(review.UserName, authState.User.Identity!.Name) == false);
				}
			}
			else
			{
				this.ToastService.ShowError(response.Message!);
			}
			
			GetUserReviewForProductResponse existingReviewResponse = await this.ReviewService.GetUserReview(authState.User.Identity?.Name ?? string.Empty, Convert.ToInt32(this.ProductId));

			if (response.Success)
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

		private void OnStarMouseOut(int starNum)
		{
			for (int i = starNum; i > 0; i--)
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

		private void OnStarClick(int starNum)
		{
			this.OnStarMouseOut(5);
			this.ReviewModel.Stars = starNum;
			
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
				this.ToastService.ShowSuccess("Review Submitted Successfully");
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
			
		}

		private void EditReviewClick()
		{
			this.EditExistingReview = true;
		}

		private async Task UpdateReview()
		{
			AuthenticationState authState = await this.AuthenticationState;
			this.ReviewModel.UserName = authState.User.Identity?.Name ?? string.Empty;
			this.ReviewModel.ProductId = Convert.ToInt32(this.ProductId);
			
			
		}
		
		
	}
}