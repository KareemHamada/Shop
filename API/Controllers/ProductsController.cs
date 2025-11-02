using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Date;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
    {


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductSpecification(productParams);

            return await CreatePagedResult(repo, spec,
                productParams.PageIndex, productParams.PageSize);

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return product;
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            repo.Add(product);
            if(await repo.SaveAllAsync())
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

            repo.Update(product);
            if(await repo.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updating a product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {

            var product = await repo.GetByIdAsync(id);

            if(product == null) 
                return NotFound();

            repo.Remove(product);

            if(await repo.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting a product");

        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();

            return Ok(await repo.ListAsync(spec));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();

            return Ok(await repo.ListAsync(spec));
        }
        
        private bool ProductExists(int id)
        {
            return repo.Exists(id);
        }

    }
}
