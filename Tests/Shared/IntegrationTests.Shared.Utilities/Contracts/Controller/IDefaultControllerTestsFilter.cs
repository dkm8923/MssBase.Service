using System;

namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsFilter
{
    Task Default_Filter_Should_Return_Active_Data();
    Task Default_Filter_Should_Return_Inactive_Data();
    Task Default_Filter_Should_Filter_Data();
    Task Default_Filter_Should_Return_Zero_Records();
    Task Default_Filter_Should_Return_Unsupported_Media_Type_Null_Request_Body();
    Task Default_Filter_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body();
    // Task Default_Filter_Should_Return_Related_Data();
    // Task Default_Filter_Should_Not_Return_Related_Data();
}
