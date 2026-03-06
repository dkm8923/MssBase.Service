namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsUpdate
{
    Task Default_Update_Should_Update_Record();
    Task Default_Update_Should_Not_Update_Record_Unique_Error();
    Task Default_Update_Should_Not_Update_Record_Required_Field_Errors();
}
