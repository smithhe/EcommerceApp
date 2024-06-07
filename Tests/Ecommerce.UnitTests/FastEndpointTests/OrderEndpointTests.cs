using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Commands.CreateOrder;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.Order.Queries.GetAllOrdersByUserId;
using Ecommerce.Application.Features.Order.Queries.GetOrderAfterSuccessfulCheckout;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Constants.Infrastructure;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.Order;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.PayPal;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class OrderEndpointTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "testuser";
        
        private OrderDto _orderDto = null!;
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        private Mock<IConfiguration> _configuration = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            this._configuration = new Mock<IConfiguration>();
            
            this._orderDto = new OrderDto
            {
                UserId = _userId,
                Status = OrderStatus.Created,
                Total = 50,
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto()
                }
            };
        }

        #region CreateOrderEndpoint Tests

        [Test]
        public async Task CreateOrderEndpoint_WithValidRequestAndWithoutErrors_ReturnsCreateOrderResponse()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = PaymentSource.PayPal,
                CartItems = new CartItemDto[] { new CartItemDto() }
            };
            
            CreateOrderResponse createOrderResponse = new CreateOrderResponse
            {
                Success = true,
                Message = OrderConstants._createSuccessMessage,
                Order = this._orderDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserIdFromToken(It.IsAny<string>())).Returns(_userId);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(createOrderResponse);
            this._mediator.Setup(m => m.Send(It.IsAny<CreatePayPalOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(new CreatePayPalOrderResponse { Success = true, RedirectUrl = "redirectUrl" });

            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createSuccessMessage));
                Assert.That(result.Order, Is.EqualTo(this._orderDto));
                Assert.That(result.RedirectUrl, Is.EqualTo("redirectUrl"));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest();
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_InvalidPaymentSource_ReturnsBadRequest()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = (PaymentSource) 1000,
                CartItems = new CartItemDto[] { new CartItemDto() }
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Invalid Request"));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_WithoutCartItems_ReturnsBadRequest()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = PaymentSource.PayPal,
                CartItems = Array.Empty<CartItemDto>()
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Invalid Request"));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = PaymentSource.PayPal,
                CartItems = new CartItemDto[] { new CartItemDto() }
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_WhenOrderNotCreated_ReturnsFailedResponse()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = PaymentSource.PayPal,
                CartItems = new CartItemDto[] { new CartItemDto() }
            };
            
            CreateOrderResponse createOrderResponse = new CreateOrderResponse
            {
                Success = false,
                Message = OrderConstants._createErrorMessage
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(createOrderResponse);
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._createErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrderEndpoint_WhenPayPalOrderNotCreated_ReturnsFailedResponse()
        {
            // Arrange
            CreateOrderApiRequest request = new CreateOrderApiRequest
            {
                PaymentSource = PaymentSource.PayPal,
                CartItems = new CartItemDto[] { new CartItemDto() }
            };
            
            CreateOrderResponse createOrderResponse = new CreateOrderResponse
            {
                Success = true,
                Message = OrderConstants._createSuccessMessage,
                Order = this._orderDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserIdFromToken(It.IsAny<string>())).Returns(_userId);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(createOrderResponse);
            this._mediator.Setup(m => m.Send(It.IsAny<CreatePayPalOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(new CreatePayPalOrderResponse { Success = false, Message = PayPalConstants._createOrderErrorMessage, RedirectUrl = "redirectUrl" });
            
            CreateOrderEndpoint endpoint = Factory.Create<CreateOrderEndpoint>(Mock.Of<ILogger<CreateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object, this._configuration.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetAllOrdersByUserIdEndpoint Tests

        [Test]
        public async Task GetAllOrdersByUserIdEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            GetAllOrdersByUserIdApiRequest request = new GetAllOrdersByUserIdApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllOrdersByUserIdQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetAllOrdersByUserIdResponse
                {
                    Success = true,
                    Message = OrderConstants._getAllOrdersSuccessMessage,
                    Orders = new List<OrderDto> { this._orderDto }
                });

            GetAllOrdersByUserIdEndpoint endpoint = Factory.Create<GetAllOrdersByUserIdEndpoint>(Mock.Of<ILogger<GetAllOrdersByUserIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllOrdersByUserIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getAllOrdersSuccessMessage));
                Assert.That(result.Orders, Is.Not.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            GetAllOrdersByUserIdApiRequest request = new GetAllOrdersByUserIdApiRequest();
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            GetAllOrdersByUserIdEndpoint endpoint = Factory.Create<GetAllOrdersByUserIdEndpoint>(Mock.Of<ILogger<GetAllOrdersByUserIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllOrdersByUserIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetAllOrdersByUserIdEndpoint_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            GetAllOrdersByUserIdApiRequest request = new GetAllOrdersByUserIdApiRequest
            {
                UserId = _userId
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetAllOrdersByUserIdQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            GetAllOrdersByUserIdEndpoint endpoint = Factory.Create<GetAllOrdersByUserIdEndpoint>(Mock.Of<ILogger<GetAllOrdersByUserIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetAllOrdersByUserIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Orders, Is.Empty);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetOrderAfterSuccessfulCheckoutEndpoint Tests

        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            GetOrderAfterSuccessfulCheckoutApiRequest request = new GetOrderAfterSuccessfulCheckoutApiRequest
            {
                Id = 1
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderAfterSuccessfulCheckoutQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetOrderAfterSuccessfulCheckoutResponse
                {
                    Success = true,
                    Message = OrderConstants._getOrderAfterSuccessfulCheckoutSuccessMessage,
                    Order = this._orderDto
                });

            GetOrderAfterSuccessfulCheckoutEndpoint endpoint = Factory.Create<GetOrderAfterSuccessfulCheckoutEndpoint>(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderAfterSuccessfulCheckoutResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderAfterSuccessfulCheckoutSuccessMessage));
                Assert.That(result.Order, Is.EqualTo(this._orderDto));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            GetOrderAfterSuccessfulCheckoutApiRequest request = new GetOrderAfterSuccessfulCheckoutApiRequest();
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            GetOrderAfterSuccessfulCheckoutEndpoint endpoint = Factory.Create<GetOrderAfterSuccessfulCheckoutEndpoint>(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderAfterSuccessfulCheckoutResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderAfterSuccessfulCheckoutEndpoint_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            GetOrderAfterSuccessfulCheckoutApiRequest request = new GetOrderAfterSuccessfulCheckoutApiRequest
            {
                Id = 1
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderAfterSuccessfulCheckoutQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            GetOrderAfterSuccessfulCheckoutEndpoint endpoint = Factory.Create<GetOrderAfterSuccessfulCheckoutEndpoint>(Mock.Of<ILogger<GetOrderAfterSuccessfulCheckoutEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderAfterSuccessfulCheckoutResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region GetOrderByIdEndpoint Tests

        [Test]
        public async Task GetOrderByIdEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            GetOrderByIdApiRequest request = new GetOrderByIdApiRequest
            {
                Id = 1
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), default(CancellationToken)))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Message = OrderConstants._getOrderByIdSuccessMessage,
                    Order = this._orderDto
                });

            GetOrderByIdEndpoint endpoint = Factory.Create<GetOrderByIdEndpoint>(Mock.Of<ILogger<GetOrderByIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderByIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._getOrderByIdSuccessMessage));
                Assert.That(result.Order, Is.EqualTo(this._orderDto));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderByIdEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            GetOrderByIdApiRequest request = new GetOrderByIdApiRequest();
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            GetOrderByIdEndpoint endpoint = Factory.Create<GetOrderByIdEndpoint>(Mock.Of<ILogger<GetOrderByIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderByIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task GetOrderByIdEndpoint_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            GetOrderByIdApiRequest request = new GetOrderByIdApiRequest
            {
                Id = 1
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            GetOrderByIdEndpoint endpoint = Factory.Create<GetOrderByIdEndpoint>(Mock.Of<ILogger<GetOrderByIdEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            GetOrderByIdResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.Order, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }

        #endregion

        #region UpdateOrderEndpoint Tests

        [Test]
        public async Task UpdateOrderEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            // Arrange
            UpdateOrderApiRequest request = new UpdateOrderApiRequest
            {
                OrderToUpdate = this._orderDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), default(CancellationToken)))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = true,
                    Message = OrderConstants._updateSuccessMessage
                });

            UpdateOrderEndpoint endpoint = Factory.Create<UpdateOrderEndpoint>(Mock.Of<ILogger<UpdateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(OrderConstants._updateSuccessMessage));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateOrderEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            UpdateOrderApiRequest request = new UpdateOrderApiRequest();
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            UpdateOrderEndpoint endpoint = Factory.Create<UpdateOrderEndpoint>(Mock.Of<ILogger<UpdateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateOrderResponse result = endpoint.Response;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task UpdateOrderEndpoint_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            UpdateOrderApiRequest request = new UpdateOrderApiRequest
            {
                OrderToUpdate = this._orderDto
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>())).ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserNameFromToken(It.IsAny<string>())).Returns(_userName);
            
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), default(CancellationToken)))
                .ThrowsAsync(new Exception());
            
            UpdateOrderEndpoint endpoint = Factory.Create<UpdateOrderEndpoint>(Mock.Of<ILogger<UpdateOrderEndpoint>>(), this._mediator.Object, this._tokenService.Object);
            
            // Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateOrderResponse result = endpoint.Response;

            // Assert
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