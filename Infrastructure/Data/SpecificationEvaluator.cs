using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SkiShop.Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.IsDistinct)
            {
                query = query.Distinct();
            }

             if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);  
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }

        public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        {
            var query = inputQuery;

            // Apply criteria to filter entities
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply order by expression
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            // Apply order by descending expression
            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply includes (commented out)
            // query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Cast query to TResult
            var selectQuery = query as IQueryable<TResult>;

            // Apply select expression
            if (specification.Select != null)
            {
                selectQuery = query.Select(specification.Select);
            }

            if (specification.IsDistinct)
            {
                selectQuery = selectQuery?.Distinct();
            }

             if (specification.IsPagingEnabled)
            {
                selectQuery = selectQuery?.Skip(specification.Skip).Take(specification.Take);  
            }
            // Return the final query
            return selectQuery ?? query.Cast<TResult>();
        }
    }
}