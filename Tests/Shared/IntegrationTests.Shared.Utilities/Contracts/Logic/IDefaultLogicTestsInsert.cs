using System;

namespace IntegrationTests.Shared.Utilities.Contracts.Logic;

public interface IDefaultLogicTestsInsert
{
    Task Default_Insert_Should_Create_Record();
    Task Default_Insert_Should_Not_Create_Record_Unique_Error();
    Task Default_Insert_Should_Not_Create_Record_Required_Field_Errors();
    Task Default_Insert_Should_Not_Create_Record_Field_Max_Length_Errors();
}
