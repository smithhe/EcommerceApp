using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Constants.Infrastructure;
using Ecommerce.PayPal.Contracts;
using Ecommerce.PayPal.Contracts.Refit;
using Ecommerce.PayPal.Models;
using Ecommerce.PayPal.Models.Requests;
using Ecommerce.PayPal.Models.Responses;
using Ecommerce.PayPal.Services;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Requests.PayPal;
using Ecommerce.Shared.Responses.PayPal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Refit;

namespace Ecommerce.UnitTests.PayPalTests
{
    [TestFixture]
    public class PaypalClientServiceTests
    {
        private OrderDto _order = new OrderDto
        {
            Id = 1,
            Total = 50.00,
            UserId = Guid.NewGuid(),
            PayPalRequestId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            CreatedDate = new DateTime(),
            OrderItems = new[]
            {
                new OrderItemDto
                {
                    Id = 1,
                    OrderId = 1,
                    Quantity = 1,
                    Price = 50.00,
                    ProductDescription = "Test Description",
                    ProductName = "Test Product",
                    ProductSku = "TestSku"
                }
            }
        };
        
        private Mock<ITokenService> _tokenServiceMock = null!;
        private Mock<IPayPalApiService> _payPalApiServiceMock = null!;
        private Mock<IConfiguration> _configurationMock = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenServiceMock = new Mock<ITokenService>();
            this._payPalApiServiceMock = new Mock<IPayPalApiService>();
            this._configurationMock = new Mock<IConfiguration>();
        }

        [Test]
        public async Task CreateOrder_WhenOrderCreatesSuccessfully_ReturnsCreatePayPalOrderResponse()
        {
            //Arrange
            CreatePayPalOrderRequest request = new CreatePayPalOrderRequest
            {
                Order = this._order,
                ReturnKey = "test"
            };
            
            string testUrl = "testurl";
            PayPalCreateOrderResponse mockPayPalResponse = new PayPalCreateOrderResponse
            {
                Links = new List<Link>
                {
                    new Link
                    {
                        Href = testUrl,
                        Rel = "payer-action"
                    }
                }
            };
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockPayPalResponse), Encoding.UTF8, "application/json")
            };
            ApiResponse<PayPalCreateOrderResponse> apiResponse = new ApiResponse<PayPalCreateOrderResponse>(responseMessage, mockPayPalResponse, new RefitSettings());

            
            this._configurationMock.Setup(c => c["PayPal:ReturnBaseUrl"]).Returns("test");
            
            this._tokenServiceMock.Setup(t => t.GetNewToken()).ReturnsAsync("test");
            this._payPalApiServiceMock.Setup(p => p.CreatePayPalOrder(It.IsAny<string>(), 
                It.IsAny<PayPalCreateOrderRequest>())).ReturnsAsync(apiResponse);
            
            PaypalClientService paypalClientService = new PaypalClientService(Mock.Of<ILogger<PaypalClientService>>(), 
                this._tokenServiceMock.Object, this._payPalApiServiceMock.Object, this._configurationMock.Object);
            
            //Act
            CreatePayPalOrderResponse result = await paypalClientService.CreateOrder(request);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(PayPalConstants._createOrderSuccessMessage));
                Assert.That(result.RedirectUrl, Is.Not.Null);
                Assert.That(result.RedirectUrl, Is.EqualTo(testUrl));
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrder_WhenOrderIsEmpty_ReturnsFailResponseWithMessage()
        {
            //Arrange
            CreatePayPalOrderRequest request = new CreatePayPalOrderRequest
            {
                Order = new OrderDto(),
                ReturnKey = "test"
            };
            
            PaypalClientService paypalClientService = new PaypalClientService(Mock.Of<ILogger<PaypalClientService>>(), 
                this._tokenServiceMock.Object, this._payPalApiServiceMock.Object, this._configurationMock.Object);
            
            //Act
            CreatePayPalOrderResponse result = await paypalClientService.CreateOrder(request);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrder_WhenReturnBaseUrlIsNotSet_ReturnsFailResponseWithMessage()
        {
            //Arrange
            CreatePayPalOrderRequest request = new CreatePayPalOrderRequest
            {
                Order = this._order,
                ReturnKey = "test"
            };
            
            this._configurationMock.Setup(c => c["PayPal:ReturnBaseUrl"]).Returns(string.Empty);
            
            PaypalClientService paypalClientService = new PaypalClientService(Mock.Of<ILogger<PaypalClientService>>(), 
                this._tokenServiceMock.Object, this._payPalApiServiceMock.Object, this._configurationMock.Object);
            
            //Act
            CreatePayPalOrderResponse result = await paypalClientService.CreateOrder(request);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
        
        [Test]
        public async Task CreateOrder_WhenPayPalReturnsUnAuthorized_ReturnsFailResponseWithMessage()
        {
            //Arrange
            CreatePayPalOrderRequest request = new CreatePayPalOrderRequest
            {
                Order = this._order,
                ReturnKey = "test"
            };
            
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            ApiException error = await ApiException.Create(new HttpRequestMessage(), HttpMethod.Post, responseMessage, new RefitSettings());

            ApiResponse<PayPalCreateOrderResponse> apiResponse = new ApiResponse<PayPalCreateOrderResponse>(
                responseMessage, null, new RefitSettings(), error);
            
            this._configurationMock.Setup(c => c["PayPal:ReturnBaseUrl"]).Returns("test");
            
            this._tokenServiceMock.Setup(t => t.GetNewToken()).ReturnsAsync("test");
            this._payPalApiServiceMock.Setup(p => p.CreatePayPalOrder(It.IsAny<string>(), 
                It.IsAny<PayPalCreateOrderRequest>())).ReturnsAsync(apiResponse);
            
            PaypalClientService paypalClientService = new PaypalClientService(Mock.Of<ILogger<PaypalClientService>>(), 
                this._tokenServiceMock.Object, this._payPalApiServiceMock.Object, this._configurationMock.Object);
            
            //Act
            CreatePayPalOrderResponse result = await paypalClientService.CreateOrder(request);
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(PayPalConstants._createOrderErrorMessage));
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.ValidationErrors, Is.Empty);
            });
        }
    }
}