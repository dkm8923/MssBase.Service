namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsInsert
{
    Task Default_Insert_Should_Create_Record();
    Task Default_Insert_Should_Return_Unsupported_Media_Type_Null_Request_Body();
    Task Default_Insert_Should_Return_Bad_Request_Blank_JSON_Obj_Request_Body();
}
