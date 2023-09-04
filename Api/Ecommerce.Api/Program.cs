using Ecommerce.Application;
using Ecommerce.FastEndpoints;
using Ecommerce.Identity;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseFastEndpoints();

app.UseAuthorization();

app.MapControllers();

app.Run();