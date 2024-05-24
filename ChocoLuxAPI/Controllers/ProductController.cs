using ChocoLuxAPI.Models;
using ChocoLuxAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        //[HttpPost("AddProduct")]
        //public async Task<IActionResult> AddProduct([FromForm]ProductViewModel products)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //        string serverFolder = "";

        //        if (products.product_image != null)
        //        {
        //            string folder = "images/";
        //            folder += Guid.NewGuid().ToString() + products.product_image.FileName;
        //            products.product_ImagePath = "/" + folder;
        //            serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folder);

        //            await products.product_image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
        //        }
        //        Product model = new Product
        //        {
        //            product_name = products.product_name,
        //            product_description = products.product_description,
        //            product_price = products.product_price,
        //            product_ImagePath = products.product_ImagePath,
                    
        //            CategoryId = products.CategoryId
        //        };               

        //        // Add the new product to the database
        //        _context.tblProducts.Add(model);
        //        await _context.SaveChangesAsync();

        //    return Ok("Product added successfully.");
        //}
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductViewModel products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string serverFolder = "";

            if (products.product_image != null)
            {
                string folder = "images/";
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(products.product_image.FileName);
                folder += uniqueFileName;
                products.product_ImagePath = "/" + folder;
                serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folder);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(serverFolder));

                // Copy the file to the server
                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    await products.product_image.CopyToAsync(fileStream);
                }
            }

            Product model = new Product
            {
                product_name = products.product_name,
                product_description = products.product_description,
                product_price = products.product_price,
                product_ImagePath = products.product_ImagePath,
                CategoryId = products.CategoryId
            };

            // Add the new product to the database
            _context.tblProducts.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Product added successfully.");
        }


        [HttpGet("EditProduct/{id}")]
        public IActionResult EditProduct(int id)
        {
            var product = _context.tblProducts.Find(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

        //[HttpPut("EditProduct/{id}")]
        //public IActionResult EditProduct(int id, Product product)
        //{
        //    if (id != product.product_id)
        //    {
        //        return BadRequest("Invalid product ID.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        _context.Update(product);
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductExists(id))
        //        {
        //            return NotFound("Product not found.");
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Ok("Product updated successfully.");
        //}

        //private bool ProductExists(int id)
        //{
        //    return _context.tblProducts.Any(p => p.product_id == id);
        //}

        //[HttpGet]
        //public IActionResult DeleteProduct(int id)
        //{
        //    var product = _context.tblProducts.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //The method directly deletes the product from the database without any confirmation step. 
        [HttpDelete("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.tblProducts.Find(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _context.tblProducts.Remove(product);
            _context.SaveChanges();

            return Ok("Product deleted successfully.");
        }       
    }
}
