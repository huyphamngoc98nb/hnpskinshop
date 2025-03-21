using Core.Entities;
using Core.Interfaces;

namespace SkiShop.Core.Interfaces
{
    public interface IGenericsRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T?> GetEntityWithSpec(ISpecification<T> spec);
        Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec);
        Task<T?> GetById(Guid id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        // Task<bool> SaveChangesAsync();
        bool Exists(Guid id);
         Task<int> CountAsync(ISpecification<T> spec);
    }
}