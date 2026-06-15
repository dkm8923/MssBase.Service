using Contract.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData;
using Dto.Common.CommonRelationalData.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MssBase.Service.Controllers.Shared;
using MssBase.Service.Shared.Authorization;
using Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace MssBase.Service.Controllers.Common
{
    
    [Route("api/common/[controller]")]
    [ApiController]
    [Tags("CommonRelationalData")]
    [AutoValidationAttribute]
    [Authorize]
    public class CommonRelationalDataController : ApiBaseController
    {
        //TODO: Global exception handling?
        private readonly ICommonRelationalDataService _commonRelationalDataService;

        public CommonRelationalDataController(ICommonRelationalDataService commonRelationalDataService)
        {
            _commonRelationalDataService = commonRelationalDataService;
        }

        // GET: api/Common/CommonRelationalData
        
        [HttpGet()]
        [RequiredPermission(UserApiPermissions.CommonRelationalDataRead)] 
        public async Task<IActionResult> GetCommonRelationalDatas([FromQuery] bool deleteCache = false, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _commonRelationalDataService.GetAll(new BaseServiceGet { DeleteCache = deleteCache, IncludeInactive = includeInactive }, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }

        // POST: api/Common/CommonRelationalData/Filter

        [HttpPost("Filter")]
        [RequiredPermission(UserApiPermissions.CommonRelationalDataRead)]
        public async Task<IActionResult> FilterCommonRelationalDatas(FilterCommonRelationalDataServiceRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var records = await _commonRelationalDataService.Filter(req, cancellationToken);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return HandleControllerException(HttpContext, ex);
            }
        }
    }
}
