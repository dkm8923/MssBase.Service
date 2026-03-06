namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsDelete
{
    Task Default_Delete_Should_Delete_Record();
    Task Default_Delete_Should_Not_Delete_Record_Id_Does_Not_Exist();
    Task Default_Delete_Should_Return_Bad_Request_Invalid_Id();
}
