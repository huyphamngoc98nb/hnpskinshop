using System.Linq.Expressions;

namespace Core.Interfaces;
// This interface defines the specification pattern for querying entities.
public interface ISpecification<T>
{
    // Criteria for filtering entities
    Expression<Func<T, bool>> Criteria { get; }

    Expression<Func<T, object>> OrderBy { get; }
    
    Expression<Func<T, object>> OrderByDescending { get; }
    // List of related entities to include in the query
    //List<Expression<Func<T, object>>> Includes { get; }

    bool IsDistinct { get; }

    int Take { get; }

    int Skip { get; }
    bool IsPagingEnabled { get; }

    IQueryable<T> ApplyCriteria(IQueryable<T> query);

    List<Expression<Func<T, object>>> Includes {get;}

    List<string> IncludeStrings {get;}
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>?  Select { get; }
}