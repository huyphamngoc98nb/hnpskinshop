using Core.Entities;

namespace SkiShop.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetProducts(string? brands, string? types, string? sort);
        Task<Product?> GetProduct(Guid id);
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
        Task<bool> SaveChangesAsync();  

        Task<IReadOnlyList<string>> GetProductBrands();
        Task<IReadOnlyList<string>> GetProductTypes();


    }
}