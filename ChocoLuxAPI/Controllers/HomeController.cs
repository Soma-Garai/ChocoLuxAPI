using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Route("index")]  //allProducts
        public IActionResult Index()
        {
            // Retrieve products from the database along with categories 
            var productsWithCategories = _appDbContext.tblProducts
                .Include(p => p.Category) // Include category information
                .Select(p => new ProductWithCategoryDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductImagePath = $"{Request.Scheme}://{Request.Host}{p.ProductImagePath}", // Construct absolute URL
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName // Assuming there's a property for CategoryName in Category entity
                })
                .ToList();
            //ViewBag.ProductsWithCategories = productsWithCategories;
            return Ok(productsWithCategories);
        }

        //public IActionResult About()
        //{

        //    return Ok();
        //}
        //get all the products

        [HttpGet]
        [Route("chocolates")]
        //does not have to be authorized to view chocolates.
        public IActionResult Chocolates([FromQuery]Guid? categoryId)
        {
            // Retrieve products from the database along with categories 
            var productsWithCategories = _appDbContext.tblProducts
                .Include(p => p.Category) // Include category information
                .Select(p => new ProductWithCategoryDto
                {
                    //product_id = p.product_id,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductImagePath = $"{Request.Scheme}://{Request.Host}{p.ProductImagePath}",
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName // Assuming there's a property for CategoryName in Category entity
                })
                .ToList();
            // Filter products based on categoryId if provided
            if (categoryId.HasValue)
            {
                productsWithCategories = productsWithCategories.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            //ViewBag.ProductsWithCategories = productsWithCategories;
            return Ok(productsWithCategories);

        }
        //public IActionResult Testimonial()
        //{
        //    return Ok();
        //}
        //public IActionResult ContactUs()
        //{

        //    return Ok();
        //}
        //[Authorize(Policy = "Home - GetCategories")]
        [HttpGet]
        [Route("api/categories")]
        public IActionResult GetCategories()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId == null)
            //{
            //    return Unauthorized("User ID not found in token.");
            //}
            var categories = _appDbContext.tblCategories.ToList(); 
            return Ok(categories);
        }
    }
}

