using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductWebApi.Data;
using ProductWebApi.Middleware;
using ProductWebApi.Models;
using ProductWebApi.Repository;
using ProductWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers

builder.Services.AddControllers();

#endregion

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

#endregion

#region Identity

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region JWT Authentication

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

builder.Services.AddAuthorization();

#endregion

#region Dependency Injection

builder.Services.AddScoped<
    IJwtService,
    JwtService>();

builder.Services.AddScoped<
    IRefreshTokenService,
    RefreshTokenService>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IProductRepository,
    ProductRepository>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Product Web API",
            Version = "v1"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter JWT Token : Bearer {token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");

app.UseSwagger();

app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();