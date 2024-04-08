using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using DataLayer.Data;
using RepositoryLayer.Interfaces;
using DataLayer.Models;
using RepositoryLayer.Repositories;
using InfrastructureLayer.Interface;
using InfrastructureLayer.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Welcome", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenValue = builder.Configuration.GetSection("AppSettings:Token").Value;
        if (tokenValue != null)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenValue)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
        else
        {
            throw new InvalidOperationException("JWT token configuration is missing or invalid.");
        }
    });
    

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers to the container
builder.Services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Registration>());


builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("villaa")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
                      builder  =>
                      {
                          builder.WithOrigins("http://localhost:4200")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();   
                      });
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository , UserRepository>();
builder.Services.AddScoped<IGoogleDriveService , GoogleDriveService>();
builder.Services.AddScoped<IVideoRepository , VideoRepository>();


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Networking"));
}

//app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
//app.UseMiddleware<JwtMiddleware>();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();
