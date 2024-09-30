using AutoMapper;
using EAD_Web_Service.Models;
using EAD_Web_Service.Services;
using EAD_Web_Service.Services.Impl;
using EAD_Web_Service.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters; // For StringEnumConverter
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT secret key
var key = builder.Configuration["Jwt:SecretKey"] ?? "B374A26A71490437AA024E4FADD5B497FDFF1A8EA6FF12F6FB65AF2720B59CCF";

// AutoMapper configuration
var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(typeof(MappingProfile));
});
var mapper = mapperConfiguration.CreateMapper();

// Add services to the container

// Use only one AddControllers() and then chain AddNewtonsoftJson()
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter()); // Convert enums to strings in JSON
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // Ignore null values
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EAD Web Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
    };
});

// Configure database and services
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IVendorService, VendorService>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EAD Web Service v1");
        c.OAuthClientId("your-client-id");
        c.OAuthAppName("EAD Web Service");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
