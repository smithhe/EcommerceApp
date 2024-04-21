using Ecommerce.Domain.Entities;
using Ecommerce.Identity.Contracts;
using Ecommerce.Identity.Models;
using Ecommerce.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.Persistence;

namespace Ecommerce.Identity
{
	public static class IdentityServiceRegistration
	{
		public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

			services.AddIdentity<EcommerceUser, IdentityRole<Guid>>(options =>
				{
					options.Password.RequiredLength = 9;
					options.Password.RequireUppercase = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireDigit = true;
				})
				.AddEntityFrameworkStores<EcommercePersistenceDbContext>()
				.AddDefaultTokenProviders();
			
			services.AddTransient<IAuthenticationService, AuthService>();
			
			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.SaveToken = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						RequireExpirationTime = true,
						ClockSkew = TimeSpan.FromMinutes(5),
						ValidIssuer = configuration["JwtSettings:Issuer"],
						ValidAudience = configuration["JwtSettings:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
					};
				});

			services.AddAuthorization(options =>
			{
				//Admin Only Policy
				options.AddPolicy(PolicyNames._adminPolicy, policy => policy.RequireRole(RoleNames._admin));
				
				//General Access Policy
				options.AddPolicy(PolicyNames._generalPolicy, policy => 
					policy.RequireAssertion(context => context.User.IsInRole(RoleNames._admin) || context.User.IsInRole(RoleNames._user)));
			});
		}
	}
}