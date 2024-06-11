using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(AppDbContext context, IWebHostEnvironment hostEnvironment, ILogger<ProductController> logger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [Route("allProducts")]  
        public IActionResult AllProducts()
        {
            // Retrieve products from the database along with categories 
            var productsWithCategories = _context.tblProducts
                .Include(p => p.Category) // Include category information
                .Select(p => new ProductWithCategoryDto
                {
                    //product_id = p.product_id,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    //ProductImagePath = p.ProductImagePath,
                    ProductImagePath = $"{Request.Scheme}://{Request.Host}{p.ProductImagePath}", // Construct absolute URL
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName // Assuming there's a property for CategoryName in Category entity
                })
                .ToList();
            //ViewBag.ProductsWithCategories = productsWithCategories;
            return Ok(productsWithCategories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            _logger.LogInformation("GetProductById method started with ID: {Id}", id);

            try
            {
                var product = await _context.tblProducts.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string productImagePath = null;
            string serverFolder = "";

            if (productDto.ProductImage != null)
            {
                string folder = "images/";
                string uniqueFileName = Guid.NewGuid().ToString() + productDto.ProductImage.FileName;
                string relativePath = Path.Combine(folder, uniqueFileName); // Use Path.Combine for relative path
                productImagePath = "/" + relativePath;

                if (_hostEnvironment.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is not set.");
                }

                serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folder, uniqueFileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(serverFolder) ?? throw new InvalidOperationException("Directory path is null"));

                // Copy the file to the server
                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    await productDto.ProductImage.CopyToAsync(fileStream);
                }
            }

            Product model = new Product
            {
                //ProductId = Guid.NewGuid(),
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductPrice = productDto.ProductPrice,
                ProductImagePath = productImagePath, // Correct path to be stored in the database
                CategoryId = productDto.CategoryId,
                CategoryName = productDto.CategoryName
            };//categories.FirstOrDefault(c => c.CategoryId == model.CategoryId);

            // Add the new product to the database
            _context.tblProducts.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Product added successfully.");
        }

        [HttpGet("EditProduct/{id}")]  //just return the product details based on selected id
        public async Task<IActionResult> EditProduct(Guid id)
        {
            _logger.LogInformation("GetProductById method started with ID: {Id}", id);

            try
            {
                var product = await _context.tblProducts.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid ProductId, [FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.tblProducts.FindAsync(ProductId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            string serverFolder = product.ProductImagePath;

            if (productDto.ProductImage != null)
            {
                string folder = "images/";
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.ProductImage.FileName);
                string newProductImagePath = Path.Combine(folder, uniqueFileName);
                serverFolder = Path.Combine(_hostEnvironment.WebRootPath, newProductImagePath);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(serverFolder) ?? throw new InvalidOperationException("Directory path is null"));

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(product.ProductImagePath))
                {
                    string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ProductImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Copy the new file to the server
                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    await productDto.ProductImage.CopyToAsync(fileStream);
                }

                product.ProductImagePath = "/" + newProductImagePath; // Update the path in the model
            }

            // Update other product properties
            product.ProductName = productDto.ProductName;
            product.ProductDescription = productDto.ProductDescription;
            product.ProductPrice = productDto.ProductPrice;
            product.CategoryId = productDto.CategoryId;

            _context.tblProducts.Update(product);
            await _context.SaveChangesAsync();

            return Ok("Product updated successfully.");
        }


        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.tblProducts.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Delete the image file if it exists
            if (!string.IsNullOrEmpty(product.ProductImagePath))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ProductImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.tblProducts.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully.");
        }



    }
}
