using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Application.Features.Review.Commands.CreateReview;
using Ecommerce.Application.Features.Review.Commands.DeleteReview;
using Ecommerce.Application.Features.Review.Commands.UpdateReview;
using Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct;
using Ecommerce.Application.Features.Review.Queries.GetUserReviewForProduct;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class ReviewTests
    {
        private const string _userName = "Test User";
        
        private Review _review = null!;
        private ReviewDto _reviewDto = null!;
        
        private Mock<IReviewAsyncRepository> _reviewRepository = null!;
        private Mock<IMediator> _mediator = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._reviewRepository = new Mock<IReviewAsyncRepository>();
            this._mediator = new Mock<IMediator>();
            
            this._review = new Review
            {
                UserName = _userName,
                ProductId = 1,
                Comments = "Test Comments",
                Stars = 4
            };
            
            this._reviewDto = new ReviewDto
            {
                UserName = _userName,
                ProductId = 1,
                Comments = "Test Comments",
                Stars = 4
            };
            
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region CreateReviewCommandHandler Tests

        [Test]
        public async Task CreateReviewCommandHandler_WithValidRequestAndNoErrors_ReturnsSuccess()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };
            
            this._reviewRepository.Setup(r => r.AddAsync(It.IsAny<Review>())).ReturnsAsync(1);
            this._reviewRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._review);
            this._reviewRepository.Setup(r => r.GetAverageRatingForProduct(It.IsAny<int>())).ReturnsAsync(3);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateProductResponse
                {
                    Success = true,
                    Message = ProductConstants._updateSuccessMessage
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createSuccessMessage));
                Assert.That(result.Review, Is.Not.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WithNoReview_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = null,
                UserName = _userName
            };
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WithNullUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = null
            };
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = string.Empty
            };
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WhenReviewExists_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this._review);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Review already exists for this product"));
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WhenProductDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Review?)null);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = false,
                    Product = null
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Product must exist"));
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WhenStarsIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._reviewDto.Stars = -1;
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Review?)null);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Number of stars must be greater than or equal to 0"));
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WhenCommentsExceeds500Characters_ReturnsFailedResponse()
        {
            //Arrange
            this._reviewDto.Comments = new string('a', 501);
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Review?)null);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Comment must not exceed 500 characters"));
            });
        }
        
        [Test]
        public async Task CreateReviewCommandHandler_WhenCreateFails_ReturnsFailedResponse()
        {
            //Arrange
            CreateReviewCommand command = new CreateReviewCommand
            {
                ReviewToCreate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Review?)null);
            this._reviewRepository.Setup(r => r.AddAsync(It.IsAny<Review>())).ReturnsAsync(-1);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            CreateReviewCommandHandler handler = new CreateReviewCommandHandler(Mock.Of<ILogger<CreateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            CreateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._createErrorMessage));
                Assert.That(result.Review, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region DeleteReviewCommandHandler Tests

        [Test]
        public async Task DeleteReviewCommandHandler_WithValidRequestAndNoErrors_ReturnsSuccess()
        {
            //Arrange
            DeleteReviewCommand command = new DeleteReviewCommand
            {
                ReviewToDelete = this._reviewDto
            };
            
            this._reviewRepository.Setup(r => r.DeleteAsync(It.IsAny<Review>())).ReturnsAsync(true);
            this._reviewRepository.Setup(r => r.GetAverageRatingForProduct(It.IsAny<int>())).ReturnsAsync(3);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateProductResponse
                {
                    Success = true,
                    Message = ProductConstants._updateSuccessMessage
                });
            
            DeleteReviewCommandHandler handler = new DeleteReviewCommandHandler(Mock.Of<ILogger<DeleteReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            DeleteReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._deleteSuccessMessage));
            });
        }
        
        [Test]
        public async Task DeleteReviewCommandHandler_WithNoReview_ReturnsFailedResponse()
        {
            //Arrange
            DeleteReviewCommand command = new DeleteReviewCommand
            {
                ReviewToDelete = null
            };
            
            DeleteReviewCommandHandler handler = new DeleteReviewCommandHandler(Mock.Of<ILogger<DeleteReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            DeleteReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._deleteErrorMessage));
            });
        }
        
        [Test]
        public async Task DeleteReviewCommandHandler_WhenDeleteFails_ReturnsFailedResponse()
        {
            //Arrange
            DeleteReviewCommand command = new DeleteReviewCommand
            {
                ReviewToDelete = this._reviewDto
            };

            this._reviewRepository.Setup(r => r.DeleteAsync(It.IsAny<Review>())).ReturnsAsync(false);
            
            DeleteReviewCommandHandler handler = new DeleteReviewCommandHandler(Mock.Of<ILogger<DeleteReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            DeleteReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._deleteErrorMessage));
            });
        }

        #endregion

        #region UpdateReviewCommandHandler Tests

        [Test]
        public async Task UpdateReviewCommandHandler_WithValidRequestAndNoErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };
            
            this._reviewRepository.Setup(r => r.UpdateAsync(It.IsAny<Review>())).ReturnsAsync(true);
            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(),It.IsAny<int>())).ReturnsAsync(this._review);
            this._reviewRepository.Setup(r => r.GetAverageRatingForProduct(It.IsAny<int>())).ReturnsAsync(3);

            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateProductResponse
                {
                    Success = true,
                    Message = ProductConstants._updateSuccessMessage
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WithNoReview_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = null,
                UserName = _userName
            };
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WithNullUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = null
            };
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = string.Empty
            };
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WhenReviewDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((Review?)null);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Review must exist to update"));
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WhenProductDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this._review);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = false,
                    Product = null
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Product must exist"));
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WhenStarsIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._reviewDto.Stars = -1;
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this._review);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Number of stars must be greater than or equal to 0"));
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WhenCommentsExceeds500Characters_ReturnsFailedResponse()
        {
            //Arrange
            this._reviewDto.Comments = new string('a', 501);
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this._review);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Is.Not.Empty);
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Comment must not exceed 500 characters"));
            });
        }
        
        [Test]
        public async Task UpdateReviewCommandHandler_WhenUpdateFails_ReturnsFailedResponse()
        {
            //Arrange
            UpdateReviewCommand command = new UpdateReviewCommand
            {
                ReviewToUpdate = this._reviewDto,
                UserName = _userName
            };

            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this._review);
            this._reviewRepository.Setup(r => r.UpdateAsync(It.IsAny<Review>())).ReturnsAsync(false);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Product = new ProductDto()
                });
            
            UpdateReviewCommandHandler handler = new UpdateReviewCommandHandler(Mock.Of<ILogger<UpdateReviewCommandHandler>>(),this._mapper, this._mediator.Object, this._reviewRepository.Object);
            
            //Act
            UpdateReviewResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetReviewsForProductQueryHandler Tests

        [Test]
        public async Task GetReviewsForProductQueryHandler_WithValidRequestAndNoErrors_ReturnsSuccess()
        {
            //Arrange
            GetReviewsForProductQuery query = new GetReviewsForProductQuery
            {
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.ListAllAsync(It.IsAny<int>())).ReturnsAsync(new List<Review>
            {
                this._review
            });

            GetReviewsForProductQueryHandler handler = new GetReviewsForProductQueryHandler(Mock.Of<ILogger<GetReviewsForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetReviewsForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getAllSuccessMessage));
                Assert.That(result.Reviews, Is.Not.Empty);
            });
        }
        
        [Test]
        public async Task GetReviewsForProductQueryHandler_WhenNoReviewsExist_ReturnsFailedResponse()
        {
            //Arrange
            GetReviewsForProductQuery query = new GetReviewsForProductQuery
            {
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.ListAllAsync(It.IsAny<int>())).ReturnsAsync(new List<Review>());

            GetReviewsForProductQueryHandler handler = new GetReviewsForProductQueryHandler(Mock.Of<ILogger<GetReviewsForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetReviewsForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getAllSuccessMessage));
                Assert.That(result.Reviews, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetReviewsForProductQueryHandler_WhenSqlReturnsNull_ReturnsFailedResponse()
        {
            //Arrange
            GetReviewsForProductQuery query = new GetReviewsForProductQuery
            {
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.ListAllAsync(It.IsAny<int>())).ReturnsAsync((IEnumerable<Review>?)null);

            GetReviewsForProductQueryHandler handler = new GetReviewsForProductQueryHandler(Mock.Of<ILogger<GetReviewsForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetReviewsForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getAllErrorMessage));
                Assert.That(result.Reviews, Is.Empty);
            });
        }

        #endregion

        #region GetUserReviewForProductQueryHandler Tests

        [Test]
        public async Task GetUserReviewForProductQueryHandler_WithValidRequestAndNoErrors_ReturnsSuccess()
        {
            //Arrange
            GetUserReviewForProductQuery query = new GetUserReviewForProductQuery
            {
                UserName = _userName,
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(this._review);

            GetUserReviewForProductQueryHandler handler = new GetUserReviewForProductQueryHandler(Mock.Of<ILogger<GetUserReviewForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetUserReviewForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewSuccessMessage));
                Assert.That(result.UserReview, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductQueryHandler_WithNullUserName_ReturnsFailedResponse()
        {
            //Arrange
            GetUserReviewForProductQuery query = new GetUserReviewForProductQuery
            {
                UserName = null,
                ProductId = 1
            };
            
            GetUserReviewForProductQueryHandler handler = new GetUserReviewForProductQueryHandler(Mock.Of<ILogger<GetUserReviewForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetUserReviewForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewErrorMessage));
                Assert.That(result.UserReview, Is.Null);
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductQueryHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            GetUserReviewForProductQuery query = new GetUserReviewForProductQuery
            {
                UserName = string.Empty,
                ProductId = 1
            };
            
            GetUserReviewForProductQueryHandler handler = new GetUserReviewForProductQueryHandler(Mock.Of<ILogger<GetUserReviewForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetUserReviewForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewErrorMessage));
                Assert.That(result.UserReview, Is.Null);
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductQueryHandler_WithNoReview_ReturnsSuccessWithNullReviewResponse()
        {
            //Arrange
            GetUserReviewForProductQuery query = new GetUserReviewForProductQuery
            {
                UserName = _userName,
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new Review { Id = -1});

            GetUserReviewForProductQueryHandler handler = new GetUserReviewForProductQueryHandler(Mock.Of<ILogger<GetUserReviewForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetUserReviewForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewSuccessMessage));
                Assert.That(result.UserReview, Is.Null);
            });
        }
        
        [Test]
        public async Task GetUserReviewForProductQueryHandler_WhenSqlReturnsNull_ReturnsFailedResponse()
        {
            //Arrange
            GetUserReviewForProductQuery query = new GetUserReviewForProductQuery
            {
                UserName = _userName,
                ProductId = 1
            };
            
            this._reviewRepository.Setup(r => r.GetUserReviewForProduct(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((Review?)null);

            GetUserReviewForProductQueryHandler handler = new GetUserReviewForProductQueryHandler(Mock.Of<ILogger<GetUserReviewForProductQueryHandler>>(),this._mapper, this._reviewRepository.Object);
            
            //Act
            GetUserReviewForProductResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ReviewConstants._getUserReviewErrorMessage));
                Assert.That(result.UserReview, Is.Null);
            });
        }

        #endregion
    }
}