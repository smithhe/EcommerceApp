using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Application.Features.Order.Commands.DeleteOrder;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Application.Features.PayPal.Commands.CancelPayPalOrder;
using Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder;
using Ecommerce.Application.Features.PayPal.Commands.CreatePayPalReturnKey;
using Ecommerce.Application.Features.PayPal.Commands.DeletePayPalReturnKey;
using Ecommerce.Application.Features.PayPal.Commands.HandlePayPalSuccess;
using Ecommerce.Application.Features.PayPal.Queries.GetOrderByReturnKey;
using Ecommerce.Domain.Constants.Infrastructure;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.PayPal.Contracts;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.PayPal;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.ApplicationTests
{
    [TestFixture]
    public class PayPalTests
    {
        private const string _returnKey = "ReturnKey";
        
        private Mock<IPaypalClientService> _paypalClientService = null!;
        private Mock<IOrderKeyRepository> _orderKeyRepository = null!;
        private Mock<IMediator> _mediator = null!;
        
        [SetUp]
        public void Setup()
        {
            this._mediator = new Mock<IMediator>();
            this._paypalClientService = new Mock<IPaypalClientService>();
            this._orderKeyRepository = new Mock<IOrderKeyRepository>();
            
        }

        #region CancelPayPalOrderCommandHandler Tests

        [Test]
        public async Task CancelPayPalOrderCommandHandler_WhenOrderExistsAndWithoutErrors_ReturnsTrue()
        {
            //Arrange
            CancelPayPalOrderCommand command = new CancelPayPalOrderCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<DeletePayPalReturnKeyCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteOrderResponse
                {
                    Success = true
                });
            
            CancelPayPalOrderCommandHandler handler = new CancelPayPalOrderCommandHandler(Mock.Of<ILogger<CancelPayPalOrderCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task CancelPayPalOrderCommandHandler_WhenOrderDoesNotExist_ReturnsFalse()
        {
            //Arrange
            CancelPayPalOrderCommand command = new CancelPayPalOrderCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((OrderDto?)null);
            
            CancelPayPalOrderCommandHandler handler = new CancelPayPalOrderCommandHandler(Mock.Of<ILogger<CancelPayPalOrderCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task CancelPayPalOrderCommandHandler_WhenOrderDeleteFails_ReturnsFalse()
        {
            //Arrange
            CancelPayPalOrderCommand command = new CancelPayPalOrderCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteOrderResponse
                {
                    Success = false
                });
            
            CancelPayPalOrderCommandHandler handler = new CancelPayPalOrderCommandHandler(Mock.Of<ILogger<CancelPayPalOrderCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task CancelPayPalOrderCommandHandler_WhenReturnKeyDeleteFails_ReturnsFalse()
        {
            //Arrange
            CancelPayPalOrderCommand command = new CancelPayPalOrderCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteOrderResponse
                {
                    Success = true
                });
            this._mediator.Setup(m => m.Send(It.IsAny<DeletePayPalReturnKeyCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            
            CancelPayPalOrderCommandHandler handler = new CancelPayPalOrderCommandHandler(Mock.Of<ILogger<CancelPayPalOrderCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CreatePayPalOrderCommandHandler Tests
        
        [Test]
        public async Task CreatePayPalOrderCommandHandler_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            CreatePayPalOrderCommand command = new CreatePayPalOrderCommand
            {
                Order = new OrderDto
                {
                    OrderItems = new []{ new OrderItemDto() }
                }
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreatePayPalReturnKeyCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("test");
            this._paypalClientService.Setup(m => m.CreateOrder(It.IsAny<CreatePayPalOrderRequest>()))
                .ReturnsAsync(new CreatePayPalOrderResponse
                {
                    Success = true,
                    Message = PayPalConstants._createOrderSuccessMessage
                });

            CreatePayPalOrderCommandHandler handler = new CreatePayPalOrderCommandHandler(Mock.Of<ILogger<CreatePayPalOrderCommandHandler>>(),this._mediator.Object, this._paypalClientService.Object);

            //Act
            CreatePayPalOrderResponse response = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.True);
                Assert.That(response.Message, Is.EqualTo(PayPalConstants._createOrderSuccessMessage));
            });
        }

        [Test]
        public async Task CreatePayPalOrderCommandHandler_WhenOrderHasNoItems_ReturnsFailedResponse()
        {
            //Arrange
            CreatePayPalOrderCommand command = new CreatePayPalOrderCommand
            {
                Order = new OrderDto
                {
                    OrderItems = new []{ new OrderItemDto() }
                }
            };

            CreatePayPalOrderCommandHandler handler = new CreatePayPalOrderCommandHandler(Mock.Of<ILogger<CreatePayPalOrderCommandHandler>>(),this._mediator.Object, this._paypalClientService.Object);

            //Act
            CreatePayPalOrderResponse response = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
            });
        }
        
        [Test]
        public async Task CreatePayPalOrderCommandHandler_WhenReturnKeyCreationFails_ReturnsFailedResponse()
        {
            //Arrange
            CreatePayPalOrderCommand command = new CreatePayPalOrderCommand
            {
                Order = new OrderDto
                {
                    OrderItems = new []{ new OrderItemDto() }
                }
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreatePayPalReturnKeyCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(string.Empty);

            CreatePayPalOrderCommandHandler handler = new CreatePayPalOrderCommandHandler(Mock.Of<ILogger<CreatePayPalOrderCommandHandler>>(),this._mediator.Object, this._paypalClientService.Object);

            //Act
            CreatePayPalOrderResponse response = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
            });
        }
        
        [Test]
        public async Task CreatePayPalOrderCommandHandler_WhenPayPalOrderCreationFails_ReturnsFailedResponse()
        {
            //Arrange
            CreatePayPalOrderCommand command = new CreatePayPalOrderCommand
            {
                Order = new OrderDto
                {
                    OrderItems = new []{ new OrderItemDto() }
                }
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<CreatePayPalReturnKeyCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("test");
            this._paypalClientService.Setup(m => m.CreateOrder(It.IsAny<CreatePayPalOrderRequest>()))
                .ReturnsAsync(new CreatePayPalOrderResponse
                {
                    Success = false,
                    Message = PayPalConstants._createOrderErrorMessage
                });
            
            CreatePayPalOrderCommandHandler handler = new CreatePayPalOrderCommandHandler(Mock.Of<ILogger<CreatePayPalOrderCommandHandler>>(),this._mediator.Object, this._paypalClientService.Object);
            
            //Act
            CreatePayPalOrderResponse response = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
            });
        }

        #endregion

        #region CreatePayPalReturnKeyCommandHandler Tests

        [Test]
        public async Task CreatePayPalReturnKeyCommandHandler_WhenOrderExistsAndWithoutErrors_ReturnsReturnKey()
        {
            //Arrange
            CreatePayPalReturnKeyCommand command = new CreatePayPalReturnKeyCommand
            {
                OrderId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto()
                });
            this._orderKeyRepository.Setup(m => m.AddAsync(It.IsAny<OrderKey>())).ReturnsAsync(1);
            
            CreatePayPalReturnKeyCommandHandler handler = new CreatePayPalReturnKeyCommandHandler(Mock.Of<ILogger<CreatePayPalReturnKeyCommandHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            string? result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public async Task CreatePayPalReturnKeyCommandHandler_WhenOrderDoesNotExist_ReturnsNull()
        {
            //Arrange
            CreatePayPalReturnKeyCommand command = new CreatePayPalReturnKeyCommand
            {
                OrderId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = false
                });
            
            CreatePayPalReturnKeyCommandHandler handler = new CreatePayPalReturnKeyCommandHandler(Mock.Of<ILogger<CreatePayPalReturnKeyCommandHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            string? result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task CreatePayPalReturnKeyCommandHandler_WhenOrderKeyCreationFails_ReturnsNull()
        {
            //Arrange
            CreatePayPalReturnKeyCommand command = new CreatePayPalReturnKeyCommand
            {
                OrderId = 1
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto()
                });
            this._orderKeyRepository.Setup(m => m.AddAsync(It.IsAny<OrderKey>())).ReturnsAsync(-1);
            
            CreatePayPalReturnKeyCommandHandler handler = new CreatePayPalReturnKeyCommandHandler(Mock.Of<ILogger<CreatePayPalReturnKeyCommandHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            string? result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region DeletePayPalReturnKeyCommandHandler Tests

        [Test]
        public async Task DeletePayPalReturnKeyCommandHandler_WhenOrderKeyExistsAndWithoutErrors_ReturnsTrue()
        {
            //Arrange
            DeletePayPalReturnKeyCommand command = new DeletePayPalReturnKeyCommand
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync(new OrderKey());
            this._orderKeyRepository.Setup(m => m.DeleteAsync(It.IsAny<OrderKey>())).ReturnsAsync(true);
            
            DeletePayPalReturnKeyCommandHandler handler = new DeletePayPalReturnKeyCommandHandler(Mock.Of<ILogger<DeletePayPalReturnKeyCommandHandler>>(), this._orderKeyRepository.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeletePayPalReturnKeyCommandHandler_WhenOrderKeyDoesNotExist_ReturnsFalse()
        {
            //Arrange
            DeletePayPalReturnKeyCommand command = new DeletePayPalReturnKeyCommand
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync((OrderKey?)null);
            
            DeletePayPalReturnKeyCommandHandler handler = new DeletePayPalReturnKeyCommandHandler(Mock.Of<ILogger<DeletePayPalReturnKeyCommandHandler>>(), this._orderKeyRepository.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task DeletePayPalReturnKeyCommandHandler_WhenOrderKeyDeleteFails_ReturnsFalse()
        {
            //Arrange
            DeletePayPalReturnKeyCommand command = new DeletePayPalReturnKeyCommand
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync(new OrderKey());
            this._orderKeyRepository.Setup(m => m.DeleteAsync(It.IsAny<OrderKey>())).ReturnsAsync(false);
            
            DeletePayPalReturnKeyCommandHandler handler = new DeletePayPalReturnKeyCommandHandler(Mock.Of<ILogger<DeletePayPalReturnKeyCommandHandler>>(), this._orderKeyRepository.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region HandlePayPalSuccessCommandHandler Tests

        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WithValidRequestAndWithoutErrors_ReturnsTrue()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = true
                });
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteUserCartItemsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteUserCartItemsResponse
                {
                    Success = true
                });
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WhenReturnKeyIsNull_ReturnsFalse()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = null
            };
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WhenReturnKeyIsEmpty_ReturnsFalse()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = string.Empty
            };
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WhenOrderDoesNotExist_ReturnsFalse()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((OrderDto?)null);
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WhenOrderUpdateFails_ReturnsFalse()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = false
                });
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public async Task HandlePayPalSuccessCommandHandler_WhenCartEmptyFails_ReturnsFalse()
        {
            //Arrange
            HandlePayPalSuccessCommand command = new HandlePayPalSuccessCommand
            {
                ReturnKey = _returnKey
            };
            
            this._mediator.Setup(x => x.Send(It.IsAny<GetOrderByReturnKeyQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OrderDto());
            this._mediator.Setup(m => m.Send(It.IsAny<UpdateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateOrderResponse
                {
                    Success = true
                });
            this._mediator.Setup(m => m.Send(It.IsAny<DeleteUserCartItemsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteUserCartItemsResponse
                {
                    Success = false
                });
            
            HandlePayPalSuccessCommandHandler handler = new HandlePayPalSuccessCommandHandler(Mock.Of<ILogger<HandlePayPalSuccessCommandHandler>>(),this._mediator.Object);
            
            //Act
            bool result = await handler.Handle(command, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetOrderByReturnKeyQueryHandler Tests

        [Test]
        public async Task GetOrderByReturnKeyQueryHandler_WhenOrderKeyExistsAndWithoutErrors_ReturnsOrder()
        {
            //Arrange
            GetOrderByReturnKeyQuery query = new GetOrderByReturnKeyQuery
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync(new OrderKey());
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = true,
                    Order = new OrderDto()
                });
            
            GetOrderByReturnKeyQueryHandler handler = new GetOrderByReturnKeyQueryHandler(Mock.Of<ILogger<GetOrderByReturnKeyQueryHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            OrderDto? result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public async Task GetOrderByReturnKeyQueryHandler_WhenReturnKeyIsNull_ReturnsNull()
        {
            //Arrange
            GetOrderByReturnKeyQuery query = new GetOrderByReturnKeyQuery
            {
                ReturnKey = null!
            };
            
            GetOrderByReturnKeyQueryHandler handler = new GetOrderByReturnKeyQueryHandler(Mock.Of<ILogger<GetOrderByReturnKeyQueryHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            OrderDto? result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetOrderByReturnKeyQueryHandler_WhenReturnKeyIsEmpty_ReturnsNull()
        {
            //Arrange
            GetOrderByReturnKeyQuery query = new GetOrderByReturnKeyQuery
            {
                ReturnKey = string.Empty
            };
            
            GetOrderByReturnKeyQueryHandler handler = new GetOrderByReturnKeyQueryHandler(Mock.Of<ILogger<GetOrderByReturnKeyQueryHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            OrderDto? result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetOrderByReturnKeyQueryHandler_WhenOrderKeyDoesNotExist_ReturnsNull()
        {
            //Arrange
            GetOrderByReturnKeyQuery query = new GetOrderByReturnKeyQuery
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync((OrderKey?)null);
            
            GetOrderByReturnKeyQueryHandler handler = new GetOrderByReturnKeyQueryHandler(Mock.Of<ILogger<GetOrderByReturnKeyQueryHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            OrderDto? result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetOrderByReturnKeyQueryHandler_WhenOrderLookupFails_ReturnsNull()
        {
            //Arrange
            GetOrderByReturnKeyQuery query = new GetOrderByReturnKeyQuery
            {
                ReturnKey = _returnKey
            };
            
            this._orderKeyRepository.Setup(m => m.GetByReturnKeyAsync(It.IsAny<string>())).ReturnsAsync(new OrderKey());
            this._mediator.Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetOrderByIdResponse
                {
                    Success = false
                });
            
            GetOrderByReturnKeyQueryHandler handler = new GetOrderByReturnKeyQueryHandler(Mock.Of<ILogger<GetOrderByReturnKeyQueryHandler>>(), this._mediator.Object, this._orderKeyRepository.Object);
            
            //Act
            OrderDto? result = await handler.Handle(query, CancellationToken.None);
            
            //Assert
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}