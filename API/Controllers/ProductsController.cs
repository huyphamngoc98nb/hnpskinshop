using API.Controllers;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiShop.Core.Interfaces;
using SkiShop.Core.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork context;

        public ProductsController(IUnitOfWork context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductSpecification(productParams);
 
            return await CreatePageResult(context.Repository<Product>(), spec, productParams.PageIndex, productParams.PageSize);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await context.Repository<Product>().GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            context.Repository<Product>().Create(product);

            if (await context.Complete())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Cannot update this product");
            }

            context.Repository<Product>().Update(product);

            if (await context.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem updating product");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var deletedProduct = await context.Repository<Product>().GetById(id);

            if (deletedProduct == null)
            {
                return NotFound();
            }

            context.Repository<Product>().Delete(deletedProduct);

            if (await context.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
        {
            var spec = new BrandListSpecification();
            var brands = await context.Repository<Product>().ListAsync(spec);
            return Ok(brands);
        }

        [HttpGet("types")] 
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductTypes()
        {
            var spec = new TypeListSpecification();
            var types = await context.Repository<Product>().ListAsync(spec);
            return Ok(types);
        }
    }
}