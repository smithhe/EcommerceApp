using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Commands.CreateProduct;
using Ecommerce.Application.Features.Product.Commands.DeleteProduct;
using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using Ecommerce.Application.Features.Product.Queries.GetProductsByCategoryId;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.Product;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Product;
using Ecommerce.Shared.Responses.Product;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class ProductEndpointTests
    {
        private const string _userName = "testuser";
        
        private ProductDto _productDto = null!;
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            
            this._productDto = new ProductDto
            {
                CategoryId = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99,
                AverageRating = 4,
                QuantityAvailable = 4,
                ImageUrl = "https://test.com/image.jpg"
            };
        }

        #region CreateProductEndpoint Tests

        [Test]
        public async Task CreateProductEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            CreateProductApiRequest request = new CreateProductApiRequest
            {
                ProductToCreate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default(CancellationToken)))
                .ReturnsAsync(new CreateProductResponse
                {
                    Success = true,
                    Message = ProductConstants._createSuccessMessage,
                    Product = this._productDto
                });
            
            CreateProductEndpoint endpoint = Factory.Create<CreateProductEndpoint>(Mock.Of<ILogger<CreateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._createSuccessMessage));
                Assert.That(result.Product, Is.EqualTo(this._productDto));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateProductEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            CreateProductApiRequest request = new CreateProductApiRequest
            {
                ProductToCreate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            CreateProductEndpoint endpoint = Factory.Create<CreateProductEndpoint>(Mock.Of<ILogger<CreateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Product, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateProductEndpoint_WithException_ReturnsError()
        {
            //Arrange
            CreateProductApiRequest request = new CreateProductApiRequest
            {
                ProductToCreate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            CreateProductEndpoint endpoint = Factory.Create<CreateProductEndpoint>(Mock.Of<ILogger<CreateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Product, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region DeleteProductEndpoint Tests

        [Test]
        public async Task DeleteProductEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            DeleteProductApiRequest request = new DeleteProductApiRequest
            {
                ProductToDelete = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default(CancellationToken)))
                .ReturnsAsync(new DeleteProductResponse
                {
                    Success = true,
                    Message = ProductConstants._deleteSuccessMessage
                });
            
            DeleteProductEndpoint endpoint = Factory.Create<DeleteProductEndpoint>(Mock.Of<ILogger<DeleteProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._deleteSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteProductEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            DeleteProductApiRequest request = new DeleteProductApiRequest
            {
                ProductToDelete = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            DeleteProductEndpoint endpoint = Factory.Create<DeleteProductEndpoint>(Mock.Of<ILogger<DeleteProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task DeleteProductEndpoint_WithException_ReturnsError()
        {
            //Arrange
            DeleteProductApiRequest request = new DeleteProductApiRequest
            {
                ProductToDelete = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            DeleteProductEndpoint endpoint = Factory.Create<DeleteProductEndpoint>(Mock.Of<ILogger<DeleteProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetAllProductsByCategoryIdEndpoint Tests

        [Test]
        public async Task GetAllProductsByCategoryIdEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetAllProductsByCategoryIdApiRequest request = new GetAllProductsByCategoryIdApiRequest
            {
                CategoryId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllProductsByCategoryIdQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetAllProductsByCategoryIdResponse
                {
                    Success = true,
                    Message = ProductConstants._getAllProductsByCategorySuccessMessage,
                    Products = new List<ProductDto> { this._productDto }
                });
            
            GetAllProductsByCategoryIdEndpoint endpoint = Factory.Create<GetAllProductsByCategoryIdEndpoint>(Mock.Of<ILogger<GetAllProductsByCategoryIdEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllProductsByCategoryIdResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getAllProductsByCategorySuccessMessage));
                Assert.That(result.Products, Is.Not.Empty);
            });
        }
        
        [Test]
        public async Task GetAllProductsByCategoryIdEndpoint_WithException_ReturnsError()
        {
            //Arrange
            GetAllProductsByCategoryIdApiRequest request = new GetAllProductsByCategoryIdApiRequest
            {
                CategoryId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllProductsByCategoryIdQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            GetAllProductsByCategoryIdEndpoint endpoint = Factory.Create<GetAllProductsByCategoryIdEndpoint>(Mock.Of<ILogger<GetAllProductsByCategoryIdEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllProductsByCategoryIdResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Products, Is.Empty);
            });
        }

        #endregion

        #region GetProductByIdEndpoint Tests

        [Test]
        public async Task GetProductByIdEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            GetProductByIdApiRequest request = new GetProductByIdApiRequest
            {
                ProductId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetProductByIdResponse
                {
                    Success = true,
                    Message = ProductConstants._getProductByIdSuccessMessage,
                    Product = this._productDto
                });
            
            GetProductByIdEndpoint endpoint = Factory.Create<GetProductByIdEndpoint>(Mock.Of<ILogger<GetProductByIdEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetProductByIdResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._getProductByIdSuccessMessage));
                Assert.That(result.Product, Is.EqualTo(this._productDto));
            });
        }
        
        [Test]
        public async Task GetProductByIdEndpoint_WithException_ReturnsError()
        {
            //Arrange
            GetProductByIdApiRequest request = new GetProductByIdApiRequest
            {
                ProductId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            GetProductByIdEndpoint endpoint = Factory.Create<GetProductByIdEndpoint>(Mock.Of<ILogger<GetProductByIdEndpoint>>(), this._mediator.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetProductByIdResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Product, Is.Null);
            });
        }

        #endregion

        #region UpdateProductEndpoint Tests

        [Test]
        public async Task UpdateProductEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateProductApiRequest request = new UpdateProductApiRequest
            {
                ProductToUpdate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default(CancellationToken)))
                .ReturnsAsync(new UpdateProductResponse
                {
                    Success = true,
                    Message = ProductConstants._updateSuccessMessage
                });
            
            UpdateProductEndpoint endpoint = Factory.Create<UpdateProductEndpoint>(Mock.Of<ILogger<UpdateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(ProductConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            UpdateProductApiRequest request = new UpdateProductApiRequest
            {
                ProductToUpdate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            UpdateProductEndpoint endpoint = Factory.Create<UpdateProductEndpoint>(Mock.Of<ILogger<UpdateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateProductResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateProductEndpoint_WithException_ReturnsError()
        {
            //Arrange
            UpdateProductApiRequest request = new UpdateProductApiRequest
            {
                ProductToUpdate = this._productDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            UpdateProductEndpoint endpoint = Factory.Create<UpdateProductEndpoint>(Mock.Of<ILogger<UpdateProductEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateProductResponse result = endpoint.Response;

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