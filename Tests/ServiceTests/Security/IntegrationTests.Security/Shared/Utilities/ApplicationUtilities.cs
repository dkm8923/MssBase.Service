using Contract.Security.Application;
using Dto.Security.Application;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Shared.Models;

namespace IntegrationTests.Security.Shared.Utilities;

public class ApplicationUtilities : IApplicationUtilities
{
    protected readonly IApplicationLogic _applicationLogic;
    public ApplicationUtilities(IApplicationLogic applicationLogic) 
    {
        _applicationLogic = applicationLogic;
    }

    public async Task ClearTestTables(IApplicationUserUtilities applicationUserUtilities)
    {
        await applicationUserUtilities.DeleteAllRecords();
        await DeleteAllRecords();
    }

    public InsertUpdateApplicationRequest ConvertApplicationDtoToInsertUpdateRequest(ApplicationDto req)
    {
        return new InsertUpdateApplicationRequest
        {
            Name = req.Name,
            Description = req.Description,
            Active = req.Active,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    {
        return new InsertUpdateApplicationRequest
        { 
            Name = LogicTestUtilities.GenerateRandomString(65),
            Description = LogicTestUtilities.GenerateRandomString(257),
            Active = true,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdateApplicationRequest CreateInsertUpdateRequestWithRandomValues(bool active = true)
    {
        return new InsertUpdateApplicationRequest
        {
            Name = LogicTestUtilities.GenerateRandomString(64),
            Description = LogicTestUtilities.GenerateRandomString(256),
            Active = active,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single application test record with randomized data for integration testing purposes.
    /// </summary>
    /// <remarks>This method is intended for use in integration tests to generate a valid application record
    /// with random values. The created record is persisted using the underlying data logic and can be used to
    /// validate test scenarios that require a application entity.</remarks>
    /// <param name="active">A value indicating whether the created application test record should be marked as active. The default is <see
    /// langword="true"/>.</param>
    /// <returns>A <see cref="ApplicationDto"/> representing the newly created application test record.</returns>
    public async Task<ApplicationDto> CreateSingleApplicationTestRecord(bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithRandomValues(active);

        var ret = await _applicationLogic.Insert(insertReq);

        ret.Errors.Should().BeNullOrEmpty("Insert of application test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Creates a single application test record with specific values provided in the request object. If the request object is null, default values will be used for all properties. The created record is persisted using the underlying data logic and the resulting application data transfer object is returned.
    /// </summary> <param name="req">An optional <see cref="InsertUpdateApplicationRequest"/> containing specific values for the application test record to be created. If null, default values will be used.</param>
    /// <returns>A <see cref="ApplicationDto"/> representing the newly created application test record with specific values.</returns>
    /// <remarks>This method is intended for use in integration tests where a application record with specific values is required. It allows for flexibility in test data setup by accepting a request object with desired property values, while also providing default values if no request is supplied.</remarks>
    public async Task<ApplicationDto> CreateSingleApplicationTestRecordWithSpecificValues(InsertUpdateApplicationRequest req = null)
    {
        var ret = new ErrorValidationResult<ApplicationDto>();

        if (req == null)
        {
            var insertReq = new InsertUpdateApplicationRequest
            {
                Name = "Test Application Name",
                Description = "Test Application Description",
                Active = true,
                CurrentUser = TestConstants.CurrentUser
            };

            ret = await _applicationLogic.Insert(insertReq);
        }
        else
        {
            req.Name = req.Name ?? "Test Application Name";
            req.Description = req.Description ?? "Test Application Description";
            req.CurrentUser = req.CurrentUser ?? TestConstants.CurrentUser;
            ret = await _applicationLogic.Insert(req);
        }

        return ret.Response;
    }


    /// <summary>
    /// Asynchronously creates a set of predefined test application records in the data store.
    /// </summary>
    /// <remarks>This method is intended for use in integration testing scenarios to populate the data
    /// store with sample application records. The created records use fixed test values and may overwrite existing data
    /// with the same identifiers.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<List<ApplicationDto>> CreateTestRecords(short numberOfRecordsToCreate = 5, bool active = true)
    {
        //create test records
        var ret = new List<ApplicationDto>();
        var recordsToCreate = new List<InsertUpdateApplicationRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSingleApplicationTestRecord(active));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    /// <remarks>This method deletes both active and inactive records. Use with caution, as this
    /// operation cannot be undone.</remarks>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public async Task DeleteAllRecords()
    {
        var recordsToDelete = await _applicationLogic.GetAll(new BaseLogicGet { IncludeInactive = true });

        foreach (var record in recordsToDelete.Response)
        {
            await _applicationLogic.Delete(record.ApplicationId);
        }
    }

    public Dictionary<string, List<string>> GetExpectedMaxLengthFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name cannot exceed 64 characters!" } },
            { "Description", new List<string> { "Description cannot exceed 256 characters!" } },
            { "CurrentUser", new List<string> { "CurrentUser cannot exceed 64 characters!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRecordDoesNotExistErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Application", new List<string> { "Record does not exist for specified ApplicationId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name is a required field!" } },
            { "CurrentUser", new List<string> { "CurrentUser is a required field!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedUniqueFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name must be unique!" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two application records are equal.
    /// </summary>
    /// <remarks>This method is typically used in test scenarios to assert that two application records have
    /// matching values for all compared properties. If any property does not match, an assertion failure is
    /// triggered.</remarks>
    /// <param name="recordA">The first application record to compare.</param>
    /// <param name="recordB">The second application record to compare against the first.</param>
    public void VerifyTestRecordValuesMatch(ApplicationDto recordA, ApplicationDto recordB)
    {
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.Name.Should().Be(recordB.Name);
        recordA.Description.Should().Be(recordB.Description);
        recordA.Active.Should().Be(recordB.Active);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        //recordA.CreatedOn.Should().Be(recordB.CreatedOn);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
        //recordA.UpdatedOn.Should().Be(recordB.UpdatedOn);
    }
}
