using System.Linq.Expressions;
using Core.Interfaces;
using SkiShop.Core.Specifications;

namespace SkiShop.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T>
    {
        private readonly Expression<Func<T, bool>> criteria;

        public BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            this.criteria = criteria;
        }

        protected BaseSpecifications() : this(null!)
        {

        }

        // Criteria for filtering entities
        public Expression<Func<T, bool>> Criteria => criteria;

        // Order by expression
        public Expression<Func<T, object>> OrderBy { get; private set; }

        // Order by descending expression
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public bool IsDistinct { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; } = [];

        public List<string> IncludeStrings { get; } = [];

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if (Criteria != null)
            {
                query = query.Where(Criteria);
            }

            return query;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        // Add order by expression
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        // Add order by descending expression
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByExpressionDescending)
        {
            OrderByDescending = orderByExpressionDescending;
        }
        //public List<Expression<Func<T, object>>> Includes => throw new NotImplementedException();

        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Take = take;
            Skip = skip;
            IsPagingEnabled = true;
        }
    }


    // Specification with select expression
    public class BaseSpecifications<T, TResult>(Expression<Func<T, bool>> criteria) : BaseSpecifications<T>(criteria), ISpecification<T, TResult>
    {
        protected BaseSpecifications() : this(null!)
        {

        }
        // Select expression
        public Expression<Func<T, TResult>>? Select { get; private set; }

        // Add select expression
        protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
        {
            Select = selectExpression;
        }
    }
}


