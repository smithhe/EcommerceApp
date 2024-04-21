using System;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.ConfirmEmail;
using Ecommerce.Application.Features.EcommerceUser.Commands.RegisterEcommerceUser;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdateEcommerceUser;
using Ecommerce.Application.Features.EcommerceUser.Commands.UpdatePassword;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.FastEndpoints.Endpoints.Security;
using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ecommerce.UnitTests.FastEndpointTests
{
    [TestFixture]
    public class SecurityEndpointTests
    {
        private static readonly Guid _userId = new Guid("095a987b-b4da-4eb6-a286-19aa3c75be53");
        private const string _userName = "testuser";
        
        private Mock<ITokenService> _tokenService = null!;
        private Mock<IMediator> _mediator = null!;
        private Mock<IAuthenticationService> _authenticationService = null!;
        private Mock<IConfiguration> _configuration = null!;
        
        [SetUp]
        public void Setup()
        {
            this._tokenService = new Mock<ITokenService>();
            this._mediator = new Mock<IMediator>();
            this._authenticationService = new Mock<IAuthenticationService>();
            this._configuration = new Mock<IConfiguration>();
        }

        #region ConfirmEmailEndpoint Tests

        [Test]
        public async Task ConfirmEmailEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            ConfirmEmailRequest request = new ConfirmEmailRequest
            {
                EmailToken = "testtoken",
                UserId = _userId.ToString()
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default(CancellationToken)))
                .ReturnsAsync(new ConfirmEmailResponse
                {
                    Success = true,
                    Message = EcommerceUserConstants._confirmEmailSuccessMessage
                });
            
            ConfirmEmailEndpoint endpoint = Factory.Create<ConfirmEmailEndpoint>(Mock.Of<ILogger<ConfirmEmailEndpoint>>(), this._mediator.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            ConfirmEmailResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(EcommerceUserConstants._confirmEmailSuccessMessage));
            });
        }
        
        [Test]
        public async Task ConfirmEmailEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            ConfirmEmailRequest request = new ConfirmEmailRequest
            {
                EmailToken = "testtoken",
                UserId = _userId.ToString()
            };
            
            this._mediator.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), default(CancellationToken)))
                .Throws(new Exception());
            
            ConfirmEmailEndpoint endpoint = Factory.Create<ConfirmEmailEndpoint>(Mock.Of<ILogger<ConfirmEmailEndpoint>>(), this._mediator.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            ConfirmEmailResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion

        #region ConfirmEmailEndpoint Tests

        [Test]
        public async Task LoginEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            AuthenticationRequest request = new AuthenticationRequest
            {
                UserName = _userName,
                Password = "testpassword"
            };
            
            this._authenticationService.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(new AuthenticateResponse
                {
                    SignInResult = SignInResponseResult.Success,
                    Token = "testtoken"
                });
            
            LoginEndpoint endpoint = Factory.Create<LoginEndpoint>(Mock.Of<ILogger<LoginEndpoint>>(), this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            AuthenticateResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SignInResult, Is.EqualTo(SignInResponseResult.Success));
                Assert.That(result.Token, Is.EqualTo("testtoken"));
            });
        }
        
        [Test]
        public async Task LoginEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            AuthenticationRequest request = new AuthenticationRequest
            {
                UserName = _userName,
                Password = "testpassword"
            };
            
            this._authenticationService.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticationRequest>()))
                .Throws(new Exception());
            
            LoginEndpoint endpoint = Factory.Create<LoginEndpoint>(Mock.Of<ILogger<LoginEndpoint>>(), this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            AuthenticateResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SignInResult, Is.EqualTo(SignInResponseResult.UnexpectedError));
                Assert.That(result.Token, Is.Null);
            });
        }
        
        [Test]
        public async Task LoginEndpoint_WithInvalidCredentials_ReturnsUnauthorized()
        {
            //Arrange
            AuthenticationRequest request = new AuthenticationRequest
            {
                UserName = _userName,
                Password = "testpassword"
            };
            
            this._authenticationService.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(new AuthenticateResponse
                {
                    SignInResult = SignInResponseResult.InvalidCredentials
                });
            
            LoginEndpoint endpoint = Factory.Create<LoginEndpoint>(Mock.Of<ILogger<LoginEndpoint>>(), this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            AuthenticateResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SignInResult, Is.EqualTo(SignInResponseResult.InvalidCredentials));
                Assert.That(result.Token, Is.Null);
            });
        }
        
        [Test]
        public async Task LoginEndpoint_WithLockedAccount_ReturnsUnauthorized()
        {
            //Arrange
            AuthenticationRequest request = new AuthenticationRequest
            {
                UserName = _userName,
                Password = "testpassword"
            };
            
            this._authenticationService.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(new AuthenticateResponse
                {
                    SignInResult = SignInResponseResult.AccountLocked
                });
            
            LoginEndpoint endpoint = Factory.Create<LoginEndpoint>(Mock.Of<ILogger<LoginEndpoint>>(), this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            AuthenticateResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SignInResult, Is.EqualTo(SignInResponseResult.AccountLocked));
                Assert.That(result.Token, Is.Null);
            });
        }
        
        [Test]
        public async Task LoginEndpoint_WithDisallowedAccount_ReturnsUnauthorized()
        {
            //Arrange
            AuthenticationRequest request = new AuthenticationRequest
            {
                UserName = _userName,
                Password = "testpassword"
            };
            
            this._authenticationService.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticationRequest>()))
                .ReturnsAsync(new AuthenticateResponse
                {
                    SignInResult = SignInResponseResult.AccountNotAllowed
                });
            
            LoginEndpoint endpoint = Factory.Create<LoginEndpoint>(Mock.Of<ILogger<LoginEndpoint>>(), this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            AuthenticateResponse result = endpoint.Response;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SignInResult, Is.EqualTo(SignInResponseResult.AccountNotAllowed));
                Assert.That(result.Token, Is.Null);
            });
        }

        #endregion

        #region LogoutEndpoint Tests

        [Test]
        public async Task LogoutEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            LogoutUserRequest request = new LogoutUserRequest
            {
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            this._authenticationService.Setup(a => a.LogoutAsync(_userName));
            
            LogoutEndpoint endpoint = Factory.Create<LogoutEndpoint>(Mock.Of<ILogger<LogoutEndpoint>>(), this._tokenService.Object, this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));

            //Assert
            this._authenticationService.Verify(a => a.LogoutAsync(_userName), Times.Once);
        }
        
        [Test]
        public async Task LogoutEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            LogoutUserRequest request = new LogoutUserRequest
            {
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            LogoutEndpoint endpoint = Factory.Create<LogoutEndpoint>(Mock.Of<ILogger<LogoutEndpoint>>(), this._tokenService.Object, this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));

            //Assert
            this._authenticationService.Verify(a => a.LogoutAsync(_userName), Times.Never);
        }
        
        [Test]
        public async Task LogoutEndpoint_WithNoUserName_ReturnsBadRequest()
        {
            //Arrange
            LogoutUserRequest request = new LogoutUserRequest
            {
                UserName = string.Empty
            };
            
            LogoutEndpoint endpoint = Factory.Create<LogoutEndpoint>(Mock.Of<ILogger<LogoutEndpoint>>(), this._tokenService.Object, this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));

            //Assert
            this._authenticationService.Verify(a => a.LogoutAsync(_userName), Times.Never);
        }
        
        [Test]
        public async Task LogoutEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            LogoutUserRequest request = new LogoutUserRequest
            {
                UserName = _userName
            };
            
            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            this._authenticationService.Setup(a => a.LogoutAsync(_userName))
                .Throws(new Exception());
            
            LogoutEndpoint endpoint = Factory.Create<LogoutEndpoint>(Mock.Of<ILogger<LogoutEndpoint>>(), this._tokenService.Object, this._authenticationService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));

            //Assert
            this._authenticationService.Verify(a => a.LogoutAsync(_userName), Times.Once);
        }

        #endregion

        #region RegisterEndpoint Tests

        [Test]
        public async Task RegisterEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            CreateUserRequest request = new CreateUserRequest
            {
                UserName = _userName,
                EmailAddress = "testemail",
                Password = "testpassword",
                FirstName = "first",
                LastName = "last"
            };

            this._mediator.Setup(m => m.Send(It.IsAny<RegisterEcommerceUserCommand>(), default(CancellationToken)))
                .ReturnsAsync(new CreateUserResponse
                {
                    Success = true,
                    ConfirmationLink = "link"
                });

            this._configuration.Setup(c => c["UIUrl"]).Returns("http://localhost:3000");
            
            RegisterEndpoint endpoint = Factory.Create<RegisterEndpoint>(Mock.Of<ILogger<RegisterEndpoint>>(), this._mediator.Object, this._configuration.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.ConfirmationLink, Is.Null);
                Assert.That(result.Errors, Is.Null);
            });
        }
        
        [Test]
        public async Task RegisterEndpoint_WithNoUIUrl_ReturnsInternalServerError()
        {
            //Arrange
            CreateUserRequest request = new CreateUserRequest
            {
                UserName = _userName,
                EmailAddress = "testemail",
                Password = "testpassword",
                FirstName = "first",
                LastName = "last"
            };

            this._configuration.Setup(c => c["UIUrl"]).Returns(string.Empty);
            
            RegisterEndpoint endpoint = Factory.Create<RegisterEndpoint>(Mock.Of<ILogger<RegisterEndpoint>>(), this._mediator.Object, this._configuration.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ConfirmationLink, Is.Null);
                Assert.That(result.Errors, Is.Not.Null);
            });
        }
        
        [Test]
        public async Task RegisterEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            CreateUserRequest request = new CreateUserRequest
            {
                UserName = _userName,
                EmailAddress = "testemail",
                Password = "testpassword",
                FirstName = "first",
                LastName = "last"
            };

            this._mediator.Setup(m => m.Send(It.IsAny<RegisterEcommerceUserCommand>(), default(CancellationToken)))
                .Throws(new Exception());

            this._configuration.Setup(c => c["UIUrl"]).Returns("http://localhost:3000");
            
            RegisterEndpoint endpoint = Factory.Create<RegisterEndpoint>(Mock.Of<ILogger<RegisterEndpoint>>(), this._mediator.Object, this._configuration.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            CreateUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ConfirmationLink, Is.Null);
                Assert.That(result.Errors, Is.Not.Null);
            });
        }
        
        #endregion

        #region UpdateEcommerceUserEndpoint Tests

        [Test]
        public async Task UpdateEcommerceUserEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdateEcommerceUserRequest request = new UpdateEcommerceUserRequest
            {
                UserName = _userName,
                UpdateUserName = "newuser",
                FirstName = "first",
                LastName = "last",
                Email = "testemail"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(_userId);

            this._mediator.Setup(m => m.Send(It.IsAny<UpdateEcommerceUserCommand>(), default(CancellationToken)))
                .ReturnsAsync(new UpdateEcommerceUserResponse
                {
                    Success = true,
                    Message = EcommerceUserConstants._updateUserSuccessMessage,
                    UpdatedAccessToken = "newtoken"
                });
            
            UpdateEcommerceUserEndpoint endpoint = Factory.Create<UpdateEcommerceUserEndpoint>(Mock.Of<ILogger<UpdateEcommerceUserEndpoint>>(), 
                this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateEcommerceUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(EcommerceUserConstants._updateUserSuccessMessage));
                Assert.That(result.UpdatedAccessToken, Is.EqualTo("newtoken"));
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            UpdateEcommerceUserRequest request = new UpdateEcommerceUserRequest
            {
                UserName = _userName,
                UpdateUserName = "newuser",
                FirstName = "first",
                LastName = "last",
                Email = "testemail"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            UpdateEcommerceUserEndpoint endpoint = Factory.Create<UpdateEcommerceUserEndpoint>(Mock.Of<ILogger<UpdateEcommerceUserEndpoint>>(), 
                this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateEcommerceUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
                Assert.That(result.UpdatedAccessToken, Is.Null);
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserEndpoint_WhenUserIdNotFound_ReturnsBadRequest()
        {
            //Arrange
            UpdateEcommerceUserRequest request = new UpdateEcommerceUserRequest
            {
                UserName = _userName,
                UpdateUserName = "newuser",
                FirstName = "first",
                LastName = "last",
                Email = "testemail"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserIdFromToken(It.IsAny<string>()))
                .Returns((Guid?)null);
            
            UpdateEcommerceUserEndpoint endpoint = Factory.Create<UpdateEcommerceUserEndpoint>(Mock.Of<ILogger<UpdateEcommerceUserEndpoint>>(), 
                this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateEcommerceUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(EcommerceUserConstants._updateUserErrorMessage));
                Assert.That(result.UpdatedAccessToken, Is.Null);
            });
        }
        
        [Test]
        public async Task UpdateEcommerceUserEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            UpdateEcommerceUserRequest request = new UpdateEcommerceUserRequest
            {
                UserName = _userName,
                UpdateUserName = "newuser",
                FirstName = "first",
                LastName = "last",
                Email = "testemail"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            this._tokenService.Setup(t => t.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(_userId);

            this._mediator.Setup(m => m.Send(It.IsAny<UpdateEcommerceUserCommand>(), default(CancellationToken)))
                .Throws(new Exception());
            
            UpdateEcommerceUserEndpoint endpoint = Factory.Create<UpdateEcommerceUserEndpoint>(Mock.Of<ILogger<UpdateEcommerceUserEndpoint>>(), 
                this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdateEcommerceUserResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
                Assert.That(result.UpdatedAccessToken, Is.Null);
            });
        }

        #endregion

        #region UpdatePasswordEndpoint Tests

        [Test]
        public async Task UpdatePasswordEndpoint_WithValidRequestAndWithoutErrors_ReturnsSuccess()
        {
            //Arrange
            UpdatePasswordRequest request = new UpdatePasswordRequest
            {
                UserName = _userName,
                CurrentPassword = "testpassword",
                NewPassword = "newpassword"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            this._mediator.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), default(CancellationToken)))
                .ReturnsAsync(new UpdatePasswordResponse
                {
                    Success = true,
                    Message = EcommerceUserConstants._updatePasswordSuccessMessage
                });
            
            UpdatePasswordEndpoint endpoint = Factory.Create<UpdatePasswordEndpoint>(Mock.Of<ILogger<UpdatePasswordEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdatePasswordResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(EcommerceUserConstants._updatePasswordSuccessMessage));
            });
        }
        
        [Test]
        public async Task UpdatePasswordEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            //Arrange
            UpdatePasswordRequest request = new UpdatePasswordRequest
            {
                UserName = _userName,
                CurrentPassword = "testpassword",
                NewPassword = "newpassword"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            UpdatePasswordEndpoint endpoint = Factory.Create<UpdatePasswordEndpoint>(Mock.Of<ILogger<UpdatePasswordEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdatePasswordResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.Null);
            });
        }
        
        [Test]
        public async Task UpdatePasswordEndpoint_WhenExceptionThrown_ReturnsInternalServerError()
        {
            //Arrange
            UpdatePasswordRequest request = new UpdatePasswordRequest
            {
                UserName = _userName,
                CurrentPassword = "testpassword",
                NewPassword = "newpassword"
            };

            this._tokenService.Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            this._mediator.Setup(m => m.Send(It.IsAny<UpdatePasswordCommand>(), default(CancellationToken)))
                .Throws(new Exception());
            
            UpdatePasswordEndpoint endpoint = Factory.Create<UpdatePasswordEndpoint>(Mock.Of<ILogger<UpdatePasswordEndpoint>>(), this._mediator.Object, this._tokenService.Object);

            //Act
            await endpoint.HandleAsync(request, default(CancellationToken));
            UpdatePasswordResponse result = endpoint.Response;
            
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Unexpected Error Occurred"));
            });
        }

        #endregion
    }
}