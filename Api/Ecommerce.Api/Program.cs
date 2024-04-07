using Ecommerce.FastEndpoints;
using Ecommerce.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
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
	dbContext.Database.EnsureCreated();
	DatabaseInitializer.MigrateDatabase(dbContext);
	DatabaseInitializer.PostMigrationUpdates(dbContext);
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Add FastEndpoints
app.UseFastEndpoints();

app.MapControllers();

app.Run();