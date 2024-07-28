using ChocoLuxAPI.Models;
using ChocoLuxAPI.Permission;
using ChocoLuxAPI.Repository;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
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
//builder.Services.AddSingleton<IActionDescriptorCollectionProvider, ActionDescriptorCollectionProvider>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
builder.Services.AddIdentity<UserModel, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

builder.Services.AddScoped<IProductRepository,ProductRepository>();
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
            };
        });

builder.Services.AddAuthorization(options =>
{
    // Get all controller types in the assembly
    var controllers = Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
        .ToList();

    foreach (var policyName in from controller in controllers
                               let controllerName = controller.Name.Replace("Controller", "")
                               let actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(m =>
                                       (typeof(IActionResult).IsAssignableFrom(m.ReturnType) ||
                                        typeof(Task<IActionResult>).IsAssignableFrom(m.ReturnType)) &&
                                       m.DeclaringType == controller)
                                   .Select(m => m.Name)
                                   .ToList()
                               from action in actions
                               let actionName = action
                               select $"{controllerName} - {actionName}")
    {
        options.AddPolicy(policyName, policy =>
            policy.RequireClaim("Permission", policyName));
        Console.WriteLine(policyName);

    }
});
//builder.Services.AddAuthorization(options =>
//{
//    //options.AddPolicy("Cart-AddItemToCart", policy =>
//    //                       policy.RequireClaim("Permission", "Cart-AddItemToCart"));
//    options.AddPolicy("Cart - AddItemToCart", policy =>
//                           policy.RequireClaim("Permission", "Cart - AddItemToCart"));
//    options.AddPolicy("Cart - CreateSessionForCart", policy =>
//                           policy.RequireClaim("Permission", "Cart - CreateSessionForCart"));
//    options.AddPolicy("Cart - GetCartItems", policy =>
//                           policy.RequireClaim("Permission", "Cart - GetCartItems"));
//    options.AddPolicy("Cart - RemoveItemFromCart", policy =>
//                           policy.RequireClaim("Permission", "Cart - RemoveItemFromCart"));
//    options.AddPolicy("Cart - RemoveFromCart", policy =>
//                           policy.RequireClaim("Permission", "Cart - RemoveFromCart"));
//    options.AddPolicy("Cart - UpdateCartItem", policy =>
//                           policy.RequireClaim("Permission", "Cart - UpdateCartItem"));
//    options.AddPolicy("Cart - ClearCart", policy =>
//                           policy.RequireClaim("Permission", "Cart - ClearCart"));
//});

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
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
