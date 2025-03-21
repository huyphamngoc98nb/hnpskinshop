using Core.Entities;
using SkiShop.Core.Interfaces;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericsRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    Task<bool> Complete();

    
}