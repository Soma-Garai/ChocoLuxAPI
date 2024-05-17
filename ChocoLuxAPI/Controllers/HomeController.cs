using ChocoLuxAPI.Models;
using ChocoLuxAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Controllers
{
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
        [Route("index")]
        public IActionResult Index()
        {
            // Retrieve products from the database along with categories 
            var productsWithCategories = _appDbContext.tblProducts
                .Include(p => p.Category) // Include category information
                .Select(p => new ProductWithCategoryViewModel
                {
                    product_id = p.product_id,
                    product_name = p.product_name,
                    product_description = p.product_description,
                    product_price = p.product_price,
                    product_ImagePath = p.product_ImagePath,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName // Assuming there's a property for CategoryName in Category entity
                })
                .ToList();
            //ViewBag.ProductsWithCategories = productsWithCategories;
            return Ok(productsWithCategories);
        }

        //public IActionResult About()
        //{

        //    return Ok();
        //}

        [HttpGet]
        [Route("chocolates")]
        public IActionResult Chocolates([FromQuery]int? categoryId)
        {
            // Retrieve products from the database along with categories 
            var productsWithCategories = _appDbContext.tblProducts
                .Include(p => p.Category) // Include category information
                .Select(p => new ProductWithCategoryViewModel
                {
                    product_id = p.product_id,
                    product_name = p.product_name,
                    product_description = p.product_description,
                    product_price = p.product_price,
                    product_ImagePath = p.product_ImagePath,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName // Assuming there's a property for CategoryName in Category entity
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

        [HttpGet]
        [Route("api/categories")]
        public IActionResult GetCategories()
        {
            var categories = _appDbContext.tblCategories.ToList(); 
            return Ok(categories);
        }
    }
}

