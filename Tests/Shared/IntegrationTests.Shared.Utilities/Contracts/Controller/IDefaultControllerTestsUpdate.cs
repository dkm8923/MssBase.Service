namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsUpdate
{
    Task Default_Update_Should_Update_Record();
    Task Default_Update_Should_Return_Unsupported_Media_Type_Null_Request_Body();
    Task Default_Update_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body();
}
