using Contract.Common;
using Data.Common;
using Data.Common.Converters;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Logic;
using Dto.Common.CommonRelationalData;
using Contract.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Logic;

namespace Logic.Common.Logic
{
    public class CommonRelationalDataLogic : ICommonRelationalDataLogic
    {
        private readonly ICommonConnectionStrings _connectionStrings;
        private readonly CommonDBContextFactory _dbContextFactory;

        public CommonRelationalDataLogic(
                            ICommonConnectionStrings connectionStrings
        )
        {
            _connectionStrings = connectionStrings;
            _dbContextFactory = new CommonDBContextFactory(_connectionStrings);
        }

        /// <summary>
        /// Retrieves a collection of common relational data based on the specified request parameters.
        /// </summary>
        public async Task<ErrorValidationResult<FilterCommonRelationalDataDto>> GetAll(BaseLogicGet req, CancellationToken cancellationToken = default)
        {
            var getAllReq = new FilterCommonRelationalDataLogicRequest { 
                IncludeInactive = req.IncludeInactive, 
                CurrentUser = req.CurrentUser, 
                IncludeRelated = req.IncludeRelated
            };
            
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var referenceTypes = await dbContext.CommonRelationalData
                    .AsNoTracking()
                    .Select(x => x.ReferenceType)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                getAllReq.ReferenceTypes = referenceTypes;

                var ret = await this.Filter(getAllReq, cancellationToken);
                return ret;
            }
        }

        /// <summary>
        /// Filters common relational data based on the specified criteria.
        /// </summary>
        public async Task<ErrorValidationResult<FilterCommonRelationalDataDto>> Filter(FilterCommonRelationalDataLogicRequest req, CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.CreateContextReadOnly())
            {
                var query = dbContext.CommonRelationalData.AsQueryable().AsNoTracking();

                query = query.ApplyIncludeInactiveFilter(req);
                query = query.ApplyAuditableFilters(req);

                if (req.ReferenceTypes != null && req.ReferenceTypes.Count > 0)
                {
                    query = query.Where(x => req.ReferenceTypes.Contains(x.ReferenceType));
                }

                return new ErrorValidationResult<FilterCommonRelationalDataDto> { Response = await query.ToDto(cancellationToken) };
            }
        }
    }
}
