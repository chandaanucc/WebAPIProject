using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using villa.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Welcome", Version = "v1" });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers to the container
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Networking"));
}


app.UseHttpsRedirection();

app.MapControllers();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
// private readonly IConfiguration? _configuration;

//     public Program (IConfiguration configuration)
//     {
//         _configuration = configuration;
//     }

//     public void ConfigureServices(IServiceCollection services)
//     {
//         // Retrieve connection string from configuration
//         string connectionString = _configuration.GetConnectionString("YourConnectionString");

//         // Use the connection string for configuring DbContext or other services
//         services.AddDbContext<YourDbContext>(options =>
//         {
//             options.UseSqlServer(connectionString);
//         });
        
//         // Other service configurations...
//     }

    // Other methods...
// }
// app.MapGet("/api/villaa/registration/login", () =>
// {
//     // Your logic for handling the login endpoint
//     // Predefined username and password
//             string predefinedUsername = "demo";
//             string predefinedPassword = "password";

//     return Results.Ok("Login Endpoint Hit!");
// }).WithName("villaa").WithOpenApi();


app.Run();
