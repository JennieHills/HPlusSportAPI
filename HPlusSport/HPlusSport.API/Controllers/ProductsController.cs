using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        //[HttpGet]
        //public IEnumerable<Products> GetAllProducts()
        //{
        //    return _context.Products.ToArray();
        //}

        //Using actionResult to return status codes
        //[HttpGet]
        //public ActionResult GetAllProducts()
        //{
        //    return Ok(_context.Products.ToArray()); 
        //}

        // recommended using Asynchronous actions
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToArrayAsync());
        }

        //[HttpGet("{id}")] 
        //public ActionResult GetProduct(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    //Error handling
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(product);
        //}

        //recommended using Asynchronous action
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            //Error handling
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Products>> AddProduct(Products product)
        {

            //error handling
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction( //helper method
              "GetProduct", //action name
              new { id = product.Id },  //create the new product with the product id
                product); //the product itself

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Products product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            _context.Entry(product).State = EntityState.Modified; //checks that product has been modified
            try
            {
                await _context.SaveChangesAsync(); //save changes that was modified
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                        return NotFound();
                }
                else 
                {
                    throw;
                }
            }
            return NoContent();
            
        }

        [HttpDelete ("{id}")]
        public async Task<ActionResult<Products>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) 
            { 
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        //delete multiple products

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery]int[] ids)
        {
            var products = new List<Products>(); //create a list of products ans store the products in that list
            foreach(var id in ids) //iterate over the ids
            {
                var product = await _context.Products.FindAsync(id); //and look for each individual product
                if (product == null)
                {
                    return NotFound(id);
                }
                products.Add(product); //if the product is not found dont delete the product instead store it in the list
            }
            _context.Products.RemoveRange(products); //remove products we have in our list
            await _context.SaveChangesAsync();

            return Ok(products);
        }



    }
}
