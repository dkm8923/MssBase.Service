namespace IntegrationTests.Shared.Utilities.Contracts.Controller;

public interface IDefaultControllerTestsGetAllIncludeRelated
{
    public Task Default_GetAll_Should_Return_Related_Active_Data();
    public Task Default_GetAll_Should_Return_Related_Inactive_Data();
    public Task Default_GetAll_Should_Not_Return_Related_Data();
}
