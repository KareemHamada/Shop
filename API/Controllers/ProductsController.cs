using Core.Entities;
using Core.Interfaces;
using Infrastructure.Date;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type,string? sort)
        {
            return Ok(await repo.GetProductsAsync(brand,type,sort));
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return product;
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            repo.AddProduct(product);
            if(await repo.SaveChangesAsync())
            {
                return CreatedAtAction("Get prodcut", new { id = product.Id }, product);
            }
            
            return BadRequest("Problem creating a product");
        } 

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Can not update this product");

            repo.UpdateProduct(product);
            if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updating a product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {

            var product = await repo.GetProductByIdAsync(id);

            if(product == null) 
                return NotFound();

            repo.DeleteProduct(product);

            if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting a product");

        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }
        
        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }

    }
}
