using System;
using Ecommerce.FastEndpoints;
using Ecommerce.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("Ecommerce_");

// Add services to the container.
builder.Services.AddCors(policy =>
{
	policy.AddPolicy("OpenCorsPolicy", opts =>
		opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
	);
});


builder.Services.AddControllers();
builder.Services.AddFastEndpointServices(builder.Configuration);

WebApplication app = builder.Build();

// Initialize the database
using (IServiceScope scope = app.Services.CreateScope())
{
	EcommercePersistenceDbContext dbContext = scope.ServiceProvider.GetRequiredService<EcommercePersistenceDbContext>();
	RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
	
	dbContext.Database.EnsureCreated();
	DatabaseInitializer.MigrateDatabase(dbContext);
	DatabaseInitializer.PostMigrationUpdates(dbContext, roleManager);
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Add FastEndpoints
app.UseFastEndpoints().UseSwaggerGen();

app.MapControllers();

app.Run();