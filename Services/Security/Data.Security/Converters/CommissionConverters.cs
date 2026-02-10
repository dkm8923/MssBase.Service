// using Data.Security.Models;
// using Dto.Commission.Commission;
// using Shared.Logic.Common;

// namespace Data.Security.Converters
// {
//     public static class CommissionConverters
//     {
//         public static CommissionDto ToDto(this Models.Commission source)
//         {
//             if (source == null)
//             {
//                 return null;
//             }

//             var target = new CommissionDto
//             {
//                 CommissionId = source.CommissionId,
//                 Active = source.Active,
//                 CreatedBy = source.CreatedBy,
//                 CreatedOn = source.CreatedOn,
//                 UpdatedBy = source.UpdatedBy,
//                 UpdatedOn = source.UpdatedOn,
//                 CommissionProcessed = source.CommissionProcessed,
//                 CycleEndDate = source.CycleEndDate,
//                 Facility = source.Facility,
//                 RegionalServiceProvider = source.RegionalServiceProvider,
//                 VehicleCount = source.VehicleCount,
//                 CompletedRouteCount = source.CompletedRouteCount,
//                 ExpectedRouteCount = source.ExpectedRouteCount,
//                 DroppedRouteCount = source.DroppedRouteCount,
//                 QuickCoverageCount = source.QuickCoverageCount,
//                 CyclePackageCount = source.CyclePackageCount,
//                 MileageCount = source.MileageCount,
//                 PackageIncentiveTier = source.PackageIncentiveTier
//             };

//             return target;
//         }

//         public static List<CommissionDto> ToDtos(this IEnumerable<Models.Commission> source)
//         {
//             if (source == null)
//             {
//                 return null;
//             }

//             var target = source.Select(src => src.ToDto()).ToList();

//             return target;
//         }

//         public static Models.Commission ToEntityOnInsert(this InsertUpdateCommissionRequest source)
//         {
//             if (source == null)
//             {
//                 return null;
//             }

//             var target = new Models.Commission
//             {
//                 Active = source.Active,
//                 CommissionProcessed = source.CommissionProcessed,
//                 CycleEndDate = source.CycleEndDate,
//                 Facility = source.Facility,
//                 RegionalServiceProvider = source.RegionalServiceProvider,
//                 VehicleCount = source.VehicleCount,
//                 CompletedRouteCount = source.CompletedRouteCount,
//                 ExpectedRouteCount = source.ExpectedRouteCount,
//                 DroppedRouteCount = source.DroppedRouteCount,
//                 QuickCoverageCount = source.QuickCoverageCount,
//                 CyclePackageCount = source.CyclePackageCount,
//                 MileageCount = source.MileageCount,
//                 PackageIncentiveTier = source.PackageIncentiveTier
//             };

//             target.CreatedOn = CommonUtilities.GetDateTimeUtcNow();
//             target.CreatedBy = source.CurrentUser;
//             target.UpdatedBy = source.CurrentUser;
//             target.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

//             return target;
//         }

//         public static Models.Commission UpdateEntityFromRequest(this Models.Commission entity, InsertUpdateCommissionRequest source)
//         {
//             if (source == null || entity == null)
//             {
//                 return null;
//             }

//             entity.Active = source.Active;
//             entity.CommissionProcessed = source.CommissionProcessed;
//             entity.CycleEndDate = source.CycleEndDate;
//             entity.Facility = source.Facility;
//             entity.RegionalServiceProvider = source.RegionalServiceProvider;
//             entity.VehicleCount = source.VehicleCount;
//             entity.CompletedRouteCount = source.CompletedRouteCount;
//             entity.ExpectedRouteCount = source.ExpectedRouteCount;
//             entity.DroppedRouteCount = source.DroppedRouteCount;
//             entity.QuickCoverageCount = source.QuickCoverageCount;
//             entity.CyclePackageCount = source.CyclePackageCount;
//             entity.MileageCount = source.MileageCount;
//             entity.PackageIncentiveTier = source.PackageIncentiveTier;

//             entity.UpdatedBy = source.CurrentUser;
//             entity.UpdatedOn = CommonUtilities.GetDateTimeUtcNow();

//             return entity;
//         }
//     }
// }