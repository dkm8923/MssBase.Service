using System;

namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsGetAll
{
    Task Default_GetAll_Should_Return_Active_Data();
    Task Default_GetAll_Should_Return_Inactive_Data();
    Task Default_GetAll_Should_Return_Zero_Records();
    // Task Default_GetAll_Should_Return_Related_Data();
    // Task Default_GetAll_Should_Not_Return_Related_Data();
}
