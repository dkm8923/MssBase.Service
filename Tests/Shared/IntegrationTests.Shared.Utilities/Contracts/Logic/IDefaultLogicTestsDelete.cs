namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsDelete
{
    Task Default_Delete_Should_Delete_Record();
    Task Default_Delete_Should_Not_Delete_Record_Invalid_Id();
}
