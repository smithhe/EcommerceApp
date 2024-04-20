using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.CreateCartItem;
using Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem;
using Ecommerce.Application.Features.CartItem.Queries.GetUserCartItems;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.CartItem;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.CartItem;
using Ecommerce.Shared.Responses.CartItem;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class CartItemEndpointTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "testuser";
        
        private CartItemDto _cartItemDto = null!;
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            
            this._cartItemDto = new CartItemDto
            {
                Id = 1,
                ProductId = 1,
                Quantity = 1,
                UserId = _userId
            };
        }

        #region CreateCartItemEndpoint Tests

        [Test]
        public async Task CreateCartItemEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            CreateCartItemApiRequest request = new CreateCartItemApiRequest
            {
                CartItemToCreate = this._cartItemDto
            };
            
            CreateCartItemResponse expectedResponse = new CreateCartItemResponse
            {
                Success = true,
                Message = CartItemConstants._createSuccessMessage,
                CartItem = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            
            CreateCartItemEndpoint endpoint = Factory.Create<CreateCartItemEndpoint>(Mock.Of<ILogger<CreateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
                Assert.That(response.CartItem, Is.EqualTo(expectedResponse.CartItem));
            });
        }
        
        [Test]
        public async Task CreateCartItemEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            CreateCartItemApiRequest request = new CreateCartItemApiRequest
            {
                CartItemToCreate = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            CreateCartItemEndpoint endpoint = Factory.Create<CreateCartItemEndpoint>(Mock.Of<ILogger<CreateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task CreateCartItemEndpoint_WithException_ReturnsInternalServerError()
        {
            // Arrange
            CreateCartItemApiRequest request = new CreateCartItemApiRequest
            {
                CartItemToCreate = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            CreateCartItemEndpoint endpoint = Factory.Create<CreateCartItemEndpoint>(Mock.Of<ILogger<CreateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region DeleteCartItemEndpoint Tests

        [Test]
        public async Task DeleteCartItemEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            DeleteCartItemApiRequest request = new DeleteCartItemApiRequest
            {
                CartItemToDelete = this._cartItemDto
            };
            
            DeleteCartItemResponse expectedResponse = new DeleteCartItemResponse
            {
                Success = true,
                Message = CartItemConstants._deleteSingleItemSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            
            DeleteCartItemEndpoint endpoint = Factory.Create<DeleteCartItemEndpoint>(Mock.Of<ILogger<DeleteCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task DeleteCartItemEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            DeleteCartItemApiRequest request = new DeleteCartItemApiRequest
            {
                CartItemToDelete = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            DeleteCartItemEndpoint endpoint = Factory.Create<DeleteCartItemEndpoint>(Mock.Of<ILogger<DeleteCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task DeleteCartItemEndpoint_WithException_ReturnsInternalServerError()
        {
            // Arrange
            DeleteCartItemApiRequest request = new DeleteCartItemApiRequest
            {
                CartItemToDelete = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            DeleteCartItemEndpoint endpoint = Factory.Create<DeleteCartItemEndpoint>(Mock.Of<ILogger<DeleteCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region DeleteUserCartItemsEndpoint Tests

        [Test]
        public async Task DeleteUserCartItemsEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            DeleteUserCartItemsApiRequest request = new DeleteUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            DeleteUserCartItemsResponse expectedResponse = new DeleteUserCartItemsResponse
            {
                Success = true,
                Message = CartItemConstants._deleteAllItemsSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteUserCartItemsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            
            DeleteUserCartItemsEndpoint endpoint = Factory.Create<DeleteUserCartItemsEndpoint>(Mock.Of<ILogger<DeleteUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task DeleteUserCartItemsEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            DeleteUserCartItemsApiRequest request = new DeleteUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            DeleteUserCartItemsEndpoint endpoint = Factory.Create<DeleteUserCartItemsEndpoint>(Mock.Of<ILogger<DeleteUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task DeleteUserCartItemsEndpoint_WithException_ReturnsInternalServerError()
        {
            // Arrange
            DeleteUserCartItemsApiRequest request = new DeleteUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteUserCartItemsCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            DeleteUserCartItemsEndpoint endpoint = Factory.Create<DeleteUserCartItemsEndpoint>(Mock.Of<ILogger<DeleteUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            DeleteUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region GetUserCartItemsEndpoint Tests

        [Test]
        public async Task GetUserCartItemsEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            GetUserCartItemsApiRequest request = new GetUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            GetUserCartItemsResponse expectedResponse = new GetUserCartItemsResponse
            {
                Success = true,
                Message = CartItemConstants._getAllItemsSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetUserCartItemsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            
            GetUserCartItemsEndpoint endpoint = Factory.Create<GetUserCartItemsEndpoint>(Mock.Of<ILogger<GetUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task GetUserCartItemsEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            GetUserCartItemsApiRequest request = new GetUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            GetUserCartItemsEndpoint endpoint = Factory.Create<GetUserCartItemsEndpoint>(Mock.Of<ILogger<GetUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task GetUserCartItemsEndpoint_WithException_ReturnsInternalServerError()
        {
            // Arrange
            GetUserCartItemsApiRequest request = new GetUserCartItemsApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetUserCartItemsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            GetUserCartItemsEndpoint endpoint = Factory.Create<GetUserCartItemsEndpoint>(Mock.Of<ILogger<GetUserCartItemsEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetUserCartItemsResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region UpdateCartItemEndpoint Tests

        [Test]
        public async Task UpdateCartItemEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            UpdateCartItemApiRequest request = new UpdateCartItemApiRequest
            {
                CartItemToUpdate = this._cartItemDto
            };
            
            UpdateCartItemResponse expectedResponse = new UpdateCartItemResponse
            {
                Success = true,
                Message = CartItemConstants._updateSuccessMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            
            UpdateCartItemEndpoint endpoint = Factory.Create<UpdateCartItemEndpoint>(Mock.Of<ILogger<UpdateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(expectedResponse.Success));
                Assert.That(response.Message, Is.EqualTo(expectedResponse.Message));
            });
        }
        
        [Test]
        public async Task UpdateCartItemEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            UpdateCartItemApiRequest request = new UpdateCartItemApiRequest
            {
                CartItemToUpdate = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            UpdateCartItemEndpoint endpoint = Factory.Create<UpdateCartItemEndpoint>(Mock.Of<ILogger<UpdateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task UpdateCartItemEndpoint_WithException_ReturnsInternalServerError()
        {
            // Arrange
            UpdateCartItemApiRequest request = new UpdateCartItemApiRequest
            {
                CartItemToUpdate = this._cartItemDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateCartItemCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            UpdateCartItemEndpoint endpoint = Factory.Create<UpdateCartItemEndpoint>(Mock.Of<ILogger<UpdateCartItemEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateCartItemResponse response = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion
    }
}