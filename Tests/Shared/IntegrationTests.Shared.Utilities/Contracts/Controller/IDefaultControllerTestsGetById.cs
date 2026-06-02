using System;

namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsGetById
{
    Task Default_GetById_Should_Return_Active_Record();
    Task Default_GetById_Should_Not_Return_Inactive_Record();
    Task Default_GetById_Should_Return_Inactive_Record();
    Task Default_GetById_Should_Return_NotFound();
    Task Default_GetById_Should_Return_Bad_Request_Invalid_Id();
    Task Default_GetById_Should_Return_Unauthorized();
    Task Default_GetById_Should_Return_Forbidden();
}

