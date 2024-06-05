using ChocoLuxAPI.Models;
using ChocoLuxAPI.Permission;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        builder => builder
//            .WithOrigins("https://localhost:7183") // Add the URL of your frontend application
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials());
//});
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
builder.Services.AddIdentity<UserModel, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
//builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
//builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var userManager = builder.Services.AddScoped<UserManager<UserModel>>();
var roleManager = builder.Services.AddScoped<SignInManager<UserModel>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["jwt:validIssuer"], // The issuer of the token
                ValidAudience = configuration["jwt:validAudience"], // The audience of the token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:secretKey"])), // Your secret key for signing tokens
                ClockSkew = TimeSpan.Zero, // Set the clock skew to zero to mitigate issues with token expiration               
            };
        });
builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminPolicy", policy =>
//        policy.RequireRole("Admin"));
//    options.AddPolicy("UserPolicy", policy =>
//        policy.RequireRole("User"));

//    options.AddPolicy("ProductCreatePolicy", policy =>
//        policy.RequireClaim("Permission", "Products.Create"));

//    options.AddPolicy("ProductEditPolicy", policy =>
//        policy.RequireClaim("Permission", "Products.Edit"));
//    options.AddPolicy("ProductDeletePolicy", policy =>
//        policy.RequireClaim("Permission", "Products.Delete"));

//    //options.AddPolicy("CheckoutPolicy", policy =>
//    //    policy.RequireRole("User")
//    //          .RequireClaim("Permission", "Orders.Create"));
//    //for OR condition use requireAssertion[To create policy with multiple requirements]
//    options.AddPolicy("CheckoutPolicy", policy =>
//        policy.RequireAssertion(context =>
//        context.User.IsInRole("User") &&
//        context.User.HasClaim(claim => claim.Type == "Permission" && claim.Value == "Orders.Create") ||
//        context.User.IsInRole("Admin")));
//    // Define more policies as needed
//});

builder.Services.AddScoped<TokenGenerator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Add this line to enable static file serving
app.UseRouting();
// Enable CORS
//app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
