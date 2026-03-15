using Shared.Data.Models;
using Shared.Models;
using Shared.Models.Contracts;

namespace Shared.Logic
{
    public static class LogicUtilities
    {
        //add logic layer specific utilities here...
        
        /// <summary>
        /// Applies Active/IncludeInactive filtering.
        /// If IncludeInactive is false, only Active records are returned.
        /// </summary>
        public static IQueryable<TEntity> ApplyIncludeInactiveFilter<TEntity, TFilter>(
            this IQueryable<TEntity> query,
            TFilter filter)
            where TEntity : AuditableEntity
            where TFilter : BaseLogicGet
        {
            if (filter is null || filter.IncludeInactive)
            {
                return query;
            }

            return query.Where(x => x.Active == true);
        }

        /// <summary>
        /// Applies filters based on auditable properties to the query. (IE: CreatedBy, CreatedOnDate, UpdatedBy, UpdatedOnDate)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ApplyAuditableFilters<TEntity, TFilter>(this IQueryable<TEntity> query, TFilter filter)
            where TEntity : AuditableEntity
            where TFilter : IAuditableFilter
        {
            if (filter is null) return query;

            if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }
                
            if (filter.CreatedOnDate.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedOn) == filter.CreatedOnDate);
            }

            if (!string.IsNullOrWhiteSpace(filter.UpdatedBy))
            {
                query = query.Where(x => x.UpdatedBy == filter.UpdatedBy);
            }
                
            if (filter.UpdatedOnDate.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime((DateTime)x.UpdatedOn) == filter.UpdatedOnDate);
            }

            return query;
        }
    }
}
