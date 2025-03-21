using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;
using SkiShop.Core.Interfaces;
using SkiShop.Infrastructure.Data;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();
    private readonly StoreContext context;

    public UnitOfWork(StoreContext context)
    {
        this.context = context;
    }
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public IGenericsRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;

        return (IGenericsRepository<TEntity>)_repositories.GetOrAdd(type, t => {
            var repositoryType = typeof(GenericsRepository<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, context) ?? 
                throw new InvalidOperationException($"Could not create repository instance for {t}");
        });
    }
}