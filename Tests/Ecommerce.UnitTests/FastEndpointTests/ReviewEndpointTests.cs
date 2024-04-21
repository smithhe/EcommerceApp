using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Review.Commands.CreateReview;
using Ecommerce.Application.Features.Review.Commands.DeleteReview;
using Ecommerce.Application.Features.Review.Commands.UpdateReview;
using Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct;
using Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.Review;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Review;
using Ecommerce.Shared.Responses.Review;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class ReviewEndpointTests
    {
        private const string _userName = "testuser";
        
        private ReviewDto _reviewDto = null!;
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            
            this._reviewDto = new ReviewDto
            {
                ProductId = 1,
                UserName = _userName,
                Stars = 3,
                Comments = "Test Review"
            };
        }

        #region CreateReviewEndpoint Tests

        [Test]
        public async Task CreateReviewEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            CreateReviewApiRequest request = new CreateReviewApiRequest
            {
                ReviewToCreate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateReviewCommand>(), default(CancellationToken)))
                .ReturnsAsync(new CreateReviewResponse
                {
                    Success = true,
                    Message = ReviewConstants._createSuccessMessage,
                    Review = this._reviewDto
                });

            CreateReviewEndpoint endpoint = Factory.Create<CreateReviewEndpoint>(Mock.Of<ILogger<CreateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createSuccessMessage));
                Assert.That(result.Review, Is.EqualTo(this._reviewDto));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            CreateReviewApiRequest request = new CreateReviewApiRequest
            {
                ReviewToCreate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            CreateReviewEndpoint endpoint = Factory.Create<CreateReviewEndpoint>(Mock.Of<ILogger<CreateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewEndpoint_WithException_ReturnsInternalServerError()
        {
            //Arrange
            CreateReviewApiRequest request = new CreateReviewApiRequest
            {
                ReviewToCreate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateReviewCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());

            CreateReviewEndpoint endpoint = Factory.Create<CreateReviewEndpoint>(Mock.Of<ILogger<CreateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region DeleteReviewEndpoint Tests

        [Test]
        public async Task DeleteReviewEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            DeleteReviewApiRequest request = new DeleteReviewApiRequest
            {
                ReviewToDelete = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteReviewCommand>(), default(CancellationToken)))
                .ReturnsAsync(new DeleteReviewResponse
                {
                    Success = true,
                    Message = ReviewConstants._deleteSuccessMessage
                });

            DeleteReviewEndpoint endpoint = Factory.Create<DeleteReviewEndpoint>(Mock.Of<ILogger<DeleteReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._deleteSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteReviewEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            DeleteReviewApiRequest request = new DeleteReviewApiRequest
            {
                ReviewToDelete = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            DeleteReviewEndpoint endpoint = Factory.Create<DeleteReviewEndpoint>(Mock.Of<ILogger<DeleteReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteReviewEndpoint_WithException_ReturnsInternalServerError()
        {
            //Arrange
            DeleteReviewApiRequest request = new DeleteReviewApiRequest
            {
                ReviewToDelete = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteReviewCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());

            DeleteReviewEndpoint endpoint = Factory.Create<DeleteReviewEndpoint>(Mock.Of<ILogger<DeleteReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetReviewsForProductEndpoint Tests

        [Test]
        public async Task GetReviewsForProductEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetReviewsForProductApiRequest request = new GetReviewsForProductApiRequest
            {
                ProductId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetReviewsForProductQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetReviewsForProductResponse
                {
                    Success = true,
                    Message = ReviewConstants._getAllSuccessMessage,
                    Reviews = new[] { this._reviewDto }
                });

            GetReviewsForProductEndpoint endpoint = Factory.Create<GetReviewsForProductEndpoint>(Mock.Of<ILogger<GetReviewsForProductEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetReviewsForProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getAllSuccessMessage));
                Assert.That(result.Reviews, Is.EquivalentTo(new[] { this._reviewDto }));
            });
        }
        
        [Test]
        public async Task GetReviewsForProductEndpoint_WithException_ReturnsInternalServerError()
        {
            //Arrange
            GetReviewsForProductApiRequest request = new GetReviewsForProductApiRequest
            {
                ProductId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetReviewsForProductQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());

            GetReviewsForProductEndpoint endpoint = Factory.Create<GetReviewsForProductEndpoint>(Mock.Of<ILogger<GetReviewsForProductEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetReviewsForProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Reviews, Is.Empty);
            });
        }

        #endregion

        #region GetUserReviewForProductEndpoint Tests

        [Test]
        public async Task GetUserReviewForProductEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetUserReviewForProductApiRequest request = new GetUserReviewForProductApiRequest
            {
                ProductId = 1,
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetUserReviewForProductQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetUserReviewForProductResponse
                {
                    Success = true,
                    Message = ReviewConstants._getUserReviewSuccessMessage,
                    UserReview = this._reviewDto
                });

            GetUserReviewForProductEndpoint endpoint = Factory.Create<GetUserReviewForProductEndpoint>(Mock.Of<ILogger<GetUserReviewForProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserReviewForProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewSuccessMessage));
                Assert.That(result.UserReview, Is.EqualTo(this._reviewDto));
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            GetUserReviewForProductApiRequest request = new GetUserReviewForProductApiRequest
            {
                ProductId = 1,
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            GetUserReviewForProductEndpoint endpoint = Factory.Create<GetUserReviewForProductEndpoint>(Mock.Of<ILogger<GetUserReviewForProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserReviewForProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.UserReview, Is.Null);
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductEndpoint_WithException_ReturnsInternalServerError()
        {
            //Arrange
            GetUserReviewForProductApiRequest request = new GetUserReviewForProductApiRequest
            {
                ProductId = 1,
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetUserReviewForProductQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());

            GetUserReviewForProductEndpoint endpoint = Factory.Create<GetUserReviewForProductEndpoint>(Mock.Of<ILogger<GetUserReviewForProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserReviewForProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.UserReview, Is.Null);
            });
        }

        #endregion

        #region UpdateReviewEndpoint Tests

        [Test]
        public async Task UpdateReviewEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateReviewApiRequest request = new UpdateReviewApiRequest
            {
                ReviewToUpdate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateReviewCommand>(), default(CancellationToken)))
                .ReturnsAsync(new UpdateReviewResponse
                {
                    Success = true,
                    Message = ReviewConstants._updateSuccessMessage
                });

            UpdateReviewEndpoint endpoint = Factory.Create<UpdateReviewEndpoint>(Mock.Of<ILogger<UpdateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            UpdateReviewApiRequest request = new UpdateReviewApiRequest
            {
                ReviewToUpdate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            UpdateReviewEndpoint endpoint = Factory.Create<UpdateReviewEndpoint>(Mock.Of<ILogger<UpdateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewEndpoint_WithException_ReturnsInternalServerError()
        {
            //Arrange
            UpdateReviewApiRequest request = new UpdateReviewApiRequest
            {
                ReviewToUpdate = this._reviewDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateReviewCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());

            UpdateReviewEndpoint endpoint = Factory.Create<UpdateReviewEndpoint>(Mock.Of<ILogger<UpdateReviewEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateReviewResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion
    }
}