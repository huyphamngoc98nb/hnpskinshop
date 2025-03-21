using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SkiShop.Core.Interfaces;

namespace SkiShop.Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext context;

        public ProductRepository(StoreContext context)
        {
            this.context = context;
        }
        public void CreateProduct(Product product)
        {
            context.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            context.Products.Remove(product);
        }

        public async Task<Product?> GetProduct(Guid id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task<IReadOnlyList<string>> GetProductBrands()
        {
            return await context.Products.Select(p => p.Brand).Distinct().ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProducts(string? brands, string? types, string? sort)
        {
            var products = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(brands))
            {
                products = products.Where(p => p.Brand == brands);
            }
            
            if (!string.IsNullOrEmpty(types))
            {
                products = products.Where(p => p.Type == types);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "priceAsc":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }
            return await context.Products.ToListAsync();
        }


        public async Task<IReadOnlyList<string>> GetProductTypes()
        {
            return await context.Products.Select(p => p.Type).Distinct().ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateProduct(Product product)
        {
            context.Entry(product).State = EntityState.Modified;
        }
    }
}