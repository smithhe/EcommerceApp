using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Features.Category.Queries.GetCategoryById;
using Ecommerce.Application.Features.Product.Commands.CreateProduct;
using Ecommerce.Application.Features.Product.Commands.DeleteProduct;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId;
using Ecommerce.Application.Features.Review.Queries.GetReviewsForProduct;
using Ecommerce.Application.Profiles;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using Ecommerce.Shared.Responses.Product;
using Ecommerce.Shared.Responses.Review;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class ProductTests
    {
        private const string _userName = "Test User";
        
        private Product _product = null!;
        private ProductDto _productDto = null!;
        
        private Mock<IProductAsyncRepository> _productRepository = null!;
        private Mock<IMediator> _mediator = null!;
        private IMapper _mapper = null!;
        
        [SetUp]
        public void Setup()
        {
            this._productRepository = new Mock<IProductAsyncRepository>();
            this._mediator = new Mock<IMediator>();
            
            this._product = new Product
            {
                Name = "Test Product", 
                Price = 100,
                Description = "Test Description",
                CategoryId = 1,
                AverageRating = 4,
                ImageUrl = "test.jpg"
            };
            
            this._productDto = new ProductDto
            {
                Name = "Test Product", 
                Price = 100,
                Description = "Test Description",
                CategoryId = 1,
                AverageRating = 4,
                ImageUrl = "test.jpg"
            };
            
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            this._mapper = config.CreateMapper();
        }

        #region CreateProductCommandHandler Tests

        [Test]
        public async Task CreateProductCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.AddAsync(It.IsAny<Product>())).ReturnsAsync(1);
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Product, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WithNullProduct_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = null,
                UserName = _userName
            };
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = string.Empty
            };
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WithNullUserName_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = null
            };
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenNameIsNull_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = null!;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Name cannot not be empty"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenNameIsEmpty_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = string.Empty;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be empty"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenNameExceedsMaxLength_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = "".PadLeft(101, 'a');
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot exceed 100 characters"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenNameIsNotUnique_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(false);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name must be unique"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenDescriptionIsNull_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = null!;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Description cannot be empty"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenDescriptionIsEmpty_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = string.Empty;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot be empty"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenDescriptionExceedsMaxLength_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = "".PadLeft(501, 'a');
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot exceed 500 characters"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenAverageRatingIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.AverageRating = -1;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Average Rating cannot be less than 0"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenQuantityAvailableIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.QuantityAvailable = -1;
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Quantity Available cannot be less than 0"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenCategoryDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = null
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Category must exist"));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateProductCommandHandler_WhenSqlOperationFails_ReturnsFailedResponse()
        {
            //Arrange
            CreateProductCommand command = new CreateProductCommand
            {
                ProductToCreate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.AddAsync(It.IsAny<Product>())).ReturnsAsync(-1);
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            CreateProductCommandHandler handler = new CreateProductCommandHandler(Mock.Of<ILogger<CreateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            CreateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
                Assert.That(result.Product, Is.Null);
            });
        }

        #endregion

        #region DeleteProductCommandHandler Tests

        [Test]
        public async Task DeleteProductCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            DeleteProductCommand command = new DeleteProductCommand
            {
                ProductToDelete = this._productDto
            };
            
            this._productRepository.Setup(p => p.DeleteAsync(It.IsAny<Product>())).ReturnsAsync(true);
            
            DeleteProductCommandHandler handler = new DeleteProductCommandHandler(Mock.Of<ILogger<DeleteProductCommandHandler>>(), this._mapper, this._productRepository.Object);
            
            //Act
            DeleteProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._deleteSuccessMessage));
            });
        }
        
        [Test]
        public async Task DeleteProductCommandHandler_WithNullProduct_ReturnsFailedResponse()
        {
            //Arrange
            DeleteProductCommand command = new DeleteProductCommand
            {
                ProductToDelete = null
            };
            
            DeleteProductCommandHandler handler = new DeleteProductCommandHandler(Mock.Of<ILogger<DeleteProductCommandHandler>>(), this._mapper, this._productRepository.Object);
            
            //Act
            DeleteProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._deleteErrorMessage));
            });
        }
        
        [Test]
        public async Task DeleteProductCommandHandler_WhenSqlOperationFails_ReturnsFailedResponse()
        {
            //Arrange
            DeleteProductCommand command = new DeleteProductCommand
            {
                ProductToDelete = this._productDto
            };
            
            this._productRepository.Setup(p => p.DeleteAsync(It.IsAny<Product>())).ReturnsAsync(false);
            
            DeleteProductCommandHandler handler = new DeleteProductCommandHandler(Mock.Of<ILogger<DeleteProductCommandHandler>>(), this._mapper, this._productRepository.Object);
            
            //Act
            DeleteProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._deleteErrorMessage));
            });
        }

        #endregion

        #region UpdateProductCommandHandler Tests

        [Test]
        public async Task UpdateProductCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WithNullProduct_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = null,
                UserName = _userName
            };
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WithEmptyUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = string.Empty
            };
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WithNullUserName_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = null
            };
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenNameIsNull_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = null!;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Name cannot not be empty"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenNameIsEmpty_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = string.Empty;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot not be empty"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenNameExceedsMaxLength_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Name = "".PadLeft(101, 'a');
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name cannot exceed 100 characters"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenNameIsNotUnique_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(false);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Name must be unique"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenDescriptionIsNull_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = null!;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(2));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot not be null"));
                Assert.That(result.ValidationErrors[1], Is.EqualTo("Description cannot be empty"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenDescriptionIsEmpty_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = string.Empty;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot be empty"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenDescriptionExceedsMaxLength_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.Description = "".PadLeft(501, 'a');
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Description cannot exceed 500 characters"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenAverageRatingIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.AverageRating = -1;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Average Rating cannot be less than 0"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenQuantityAvailableIsLessThanZero_ReturnsFailedResponse()
        {
            //Arrange
            this._productDto.QuantityAvailable = -1;
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Quantity Available cannot be less than 0"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenCategoryDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = null
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._genericValidationErrorMessage));
                Assert.That(result.ValidationErrors, Has.Count.EqualTo(1));
                Assert.That(result.ValidationErrors[0], Is.EqualTo("Category must exist"));
            });
        }
        
        [Test]
        public async Task UpdateProductCommandHandler_WhenSqlOperationFails_ReturnsFailedResponse()
        {
            //Arrange
            UpdateProductCommand command = new UpdateProductCommand
            {
                ProductToUpdate = this._productDto,
                UserName = _userName
            };
            
            this._productRepository.Setup(p => p.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(false);
            this._productRepository.Setup(p => p.IsNameUnique(It.IsAny<string>())).ReturnsAsync(true);
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCategoryByIdResponse
                {
                    Category = new CategoryDto
                    {
                        Id = 1, 
                        Name = "Test Category"
                    }
                });
            
            UpdateProductCommandHandler handler = new UpdateProductCommandHandler(Mock.Of<ILogger<UpdateProductCommandHandler>>(), this._mapper, this._productRepository.Object, this._mediator.Object);
            
            //Act
            UpdateProductResponse result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateErrorMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetProductByIdQueryHandler Tests

        [Test]
        public async Task GetProductByIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetProductByIdQuery query = new GetProductByIdQuery
            {
                Id = 1
            };
            
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetReviewsForProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetReviewsForProductResponse()
                {
                    Success = true,
                    Reviews = new []{ new ReviewDto() }
                });
            
            GetProductByIdQueryHandler handler = new GetProductByIdQueryHandler(Mock.Of<ILogger<GetProductByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._productRepository.Object);
            
            //Act
            GetProductByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getProductByIdSuccessMessage));
                Assert.That(result.Product, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task GetProductByIdQueryHandler_WhenProductDoesNotExist_ReturnsFailedResponse()
        {
            //Arrange
            GetProductByIdQuery query = new GetProductByIdQuery
            {
                Id = 1
            };
            
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);
            
            GetProductByIdQueryHandler handler = new GetProductByIdQueryHandler(Mock.Of<ILogger<GetProductByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._productRepository.Object);
            
            //Act
            GetProductByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getProductByIdErrorMessage));
                Assert.That(result.Product, Is.Null);
            });
        }
        
        [Test]
        public async Task GetProductByIdQueryHandler_WhenRetrievingReviewsFails_ReturnsSuccessWithoutReviews()
        {
            //Arrange
            GetProductByIdQuery query = new GetProductByIdQuery
            {
                Id = 1
            };
            
            this._productRepository.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(this._product);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetReviewsForProductQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetReviewsForProductResponse()
                {
                    Success = false,
                    Reviews = null!
                });
            
            GetProductByIdQueryHandler handler = new GetProductByIdQueryHandler(Mock.Of<ILogger<GetProductByIdQueryHandler>>(), this._mapper, this._mediator.Object, this._productRepository.Object);
            
            //Act
            GetProductByIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getProductByIdReviewsNotFoundErrorMessage));
                Assert.That(result.Product, Is.Not.Null);
                Assert.That(result.Product!.CustomerReviews, Is.Empty);
            });
        }

        #endregion

        #region GetAllProductsByCategoryIdQueryHandler Tests

        [Test]
        public async Task GetAllProductsByCategoryIdQueryHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetAllProductsByCategoryIdQuery query = new GetAllProductsByCategoryIdQuery
            {
                CategoryId = 1
            };

            this._productRepository.Setup(p => p.ListAllAsync(It.IsAny<int>())).ReturnsAsync(new[] { this._product });
            
            GetAllProductsByCategoryIdQueryHandler handler = new GetAllProductsByCategoryIdQueryHandler(Mock.Of<ILogger<GetAllProductsByCategoryIdQueryHandler>>(), this._mapper, this._productRepository.Object);
            
            //Act
            GetAllProductsByCategoryIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getAllProductsByCategorySuccessMessage));
                Assert.That(result.Products, Is.Not.Empty);
            });
        }
        
        [Test]
        public async Task GetAllProductsByCategoryIdQueryHandler_WhenNoProductsFound_ReturnsFailedResponse()
        {
            //Arrange
            GetAllProductsByCategoryIdQuery query = new GetAllProductsByCategoryIdQuery
            {
                CategoryId = 1
            };

            this._productRepository.Setup(p => p.ListAllAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Product>());
            
            GetAllProductsByCategoryIdQueryHandler handler = new GetAllProductsByCategoryIdQueryHandler(Mock.Of<ILogger<GetAllProductsByCategoryIdQueryHandler>>(), this._mapper, this._productRepository.Object);
            
            //Act
            GetAllProductsByCategoryIdResponse result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getAllProductsByCategoryErrorMessage));
                Assert.That(result.Products, Is.Empty);
            });
        }

        #endregion
    }
}