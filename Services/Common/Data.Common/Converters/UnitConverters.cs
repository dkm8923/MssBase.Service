using Data.Common.Models;
using Dto.Common.Unit;
//using Dto.Common.UnitGroupColumn;
using Shared.Logic.Common;

namespace Data.Common.Converters
{
    public static class UnitConverters
    {
        public static UnitDto ToDto(this Unit source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new UnitDto
            {
                UnitId = source.UnitId,
                Active = source.Active,
                CreatedBy = source.CreatedBy,
                CreatedOn = source.CreatedOn,
                UpdatedBy = source.UpdatedBy,
                UpdatedOn = source.UpdatedOn,
                UnitCode = source.UnitCode,
                UnitName = source.UnitName,
                UnitDescription = source.UnitDescription,
                OriginSystem = source.OriginSystem,
                UnitDefinitionIdUnitQty = source.UnitDefinitionIdUnitQty,
                UnitDefinitionIdUnitValue = source.UnitDefinitionIdUnitValue,
                ValueTypeName = source.ValueTypeName,
                UnitPrepQuery = source.UnitPrepQuery,
                UnitHeaderQuery = source.UnitHeaderQuery,
                UnitUpdateQuery = source.UnitUpdateQuery,
                ChargeCode = source.ChargeCode
            };

            // if (source.UnitGroupColumns != null && source.UnitGroupColumns.Any()) 
            // {
            //     target.UnitGroupColumns = source.UnitGroupColumns.Select(ugc => ugc.ToDto()).ToList();
            // }
            // else
            // {
            //     target.UnitGroupColumns = new List<UnitGroupColumnDto>();
            // }

            //target.UnitGroupColumns = new List<UnitGroupColumnDto>();

            return target;
        }

        public static List<UnitDto> ToDtos(this IEnumerable<Unit> source)
        {
            if (source == null)
            {
                return null;
            }

            var target = source.Select(src => src.ToDto()).ToList();

            return target;
        }

        public static Unit ToEntityOnInsert(this InsertUpdateUnitRequest source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new Unit
            {
                Active = source.Active,
                UnitCode = source.UnitCode,
                UnitName = source.UnitName,
                UnitDescription = source.UnitDescription,
                OriginSystem = source.OriginSystem,
                UnitDefinitionIdUnitQty = source.UnitDefinitionIdUnitQty,
                UnitDefinitionIdUnitValue = source.UnitDefinitionIdUnitValue,
                ValueTypeName = source.ValueTypeName,
                UnitPrepQuery = source.UnitPrepQuery,
                UnitHeaderQuery = source.UnitHeaderQuery,
                UnitUpdateQuery = source.UnitUpdateQuery,
                ChargeCode = source.ChargeCode
            };

            target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
            target.CreatedBy = source.CurrentUser;
            target.UpdatedBy = source.CurrentUser;
            target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return target;
        }

        public static Unit UpdateEntityFromRequest(this Unit entity, InsertUpdateUnitRequest source)
        {
            if (source == null || entity == null)
            {
                return null;
            }

            entity.Active = source.Active;
            entity.UnitCode = source.UnitCode;
            entity.UnitName = source.UnitName;
            entity.UnitDescription = source.UnitDescription;
            entity.OriginSystem = source.OriginSystem;
            entity.UnitDefinitionIdUnitQty = source.UnitDefinitionIdUnitQty;
            entity.UnitDefinitionIdUnitValue = source.UnitDefinitionIdUnitValue;
            entity.ValueTypeName = source.ValueTypeName;
            entity.UnitPrepQuery = source.UnitPrepQuery;
            entity.UnitHeaderQuery = source.UnitHeaderQuery;
            entity.UnitUpdateQuery = source.UnitUpdateQuery;
            entity.ChargeCode = source.ChargeCode;

            entity.UpdatedBy = source.CurrentUser;
            entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

            return entity;
        }
    }
}
