using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.EcommerceUser;
using MediatR;
using System;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.UpdateEcommerceUser
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="EcommerceUser"/>
	/// </summary>
	public class UpdateEcommerceUserCommand : IRequest<UpdateEcommerceUserResponse>
	{
		/// <summary>
		/// The unique identifier of the User to update
		/// </summary>
		public Guid UserId { get; set; }
		
		/// <summary>
		/// The UserName of the User to update
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The First Name of the User to update
		/// </summary>
		public string? FirstName { get; set; }
		
		/// <summary>
		/// The Last Name of the User to update
		/// </summary>
		public string? LastName { get; set; }
		
	}
}