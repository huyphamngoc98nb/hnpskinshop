using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SkiShop.Core.Interfaces;

namespace SkiShop.Infrastructure.Data
{
    public class GenericsRepository<T> : IGenericsRepository<T> where T : BaseEntity
    {
        private readonly StoreContext context;

        public GenericsRepository(StoreContext context)
        {
            this.context = context;
        }

        // Create a new entity
        public void Create(T entity)
        {
            context.Set<T>().Add(entity);
        }

        // Delete an existing entity
        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        // Get all entities
        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }

        // Get an entity by its ID
        public async Task<T?> GetById(Guid id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        // Update an existing entity
        public void Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        // Check if an entity exists by its ID
        public bool Exists(Guid id)
        {
            return context.Set<T>().Any(e => e.Id == id);
        }

        // Apply specification to the query
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), spec);
        }

        // Get an entity with specification
        public async Task<T?> GetEntityWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        // List entities with specification
        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

         private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
        {
            return SpecificationEvaluator<T>.GetQuery<T, TResult>(context.Set<T>().AsQueryable(), spec);
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            var query = context.Set<T>().AsQueryable();

            query = spec.ApplyCriteria(query);
            
            return await query.CountAsync();
        }
    }
}