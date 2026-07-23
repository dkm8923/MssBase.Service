using Contract.Security.Application;
using Contract.Security.Permission;
using Dto.Security.Permission;
using FluentAssertions;
using IntegrationTests.Security.Shared.Utilities.Contracts;
using IntegrationTests.Shared;
using IntegrationTests.Shared.Utilities;
using Contract.Security;
using Data.Security;
using Data.Security.Converters;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Security.Shared.Utilities;

public class PermissionUtilities : IPermissionUtilities
{
    private readonly ISecurityConnectionStrings _connectionStrings;
    private readonly SecurityDBContextFactory _dbContextFactory;
    protected readonly IPermissionLogic _PermissionLogic;
    protected readonly IApplicationLogic _applicationLogic;
    
    public PermissionUtilities(ISecurityConnectionStrings connectionStrings, IPermissionLogic PermissionLogic, IApplicationLogic applicationLogic) 
    {
        _connectionStrings = connectionStrings;
        _dbContextFactory = new SecurityDBContextFactory(_connectionStrings);
        _PermissionLogic = PermissionLogic;
        _applicationLogic = applicationLogic;
    }

    public InsertUpdatePermissionRequest ConvertPermissionDtoToInsertUpdateRequest(PermissionDto req)
    {
        return new InsertUpdatePermissionRequest
        {
            Active = req.Active,
            Name = req.Name,
            Description = req.Description,
            ApplicationId = req.ApplicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    public InsertUpdatePermissionRequest CreateInsertUpdateRequestWithMaxLengthErrors()
    {
        return new InsertUpdatePermissionRequest
        { 
            Name = LogicTestUtilities.GenerateRandomString(65),
            Description = LogicTestUtilities.GenerateRandomString(257),
            Active = true,
            ApplicationId = 1,
            CurrentUser = LogicTestUtilities.GenerateRandomString(65)
        };
    }
    
    public InsertUpdatePermissionRequest CreateInsertUpdateRequestWithRandomValues(int applicationId, bool active = true)
    {
        return new InsertUpdatePermissionRequest
        {
            Name = LogicTestUtilities.GenerateRandomString(64),
            Description = LogicTestUtilities.GenerateRandomString(32),
            Active = active,
            ApplicationId = applicationId,
            CurrentUser = TestConstants.CurrentUser
        };
    }

    /// <summary>
    /// Creates a single permission test record with randomized data for integration testing purposes.
    /// </summary>
    public async Task<PermissionDto> CreateSinglePermissionTestRecord(int applicationId, bool active = true)
    {
        //create test record
        var insertReq = CreateInsertUpdateRequestWithRandomValues(applicationId, active);

        var ret = await _PermissionLogic.Insert(insertReq, _applicationLogic);

        ret.Errors.Should().BeNullOrEmpty("Insert of permission test record failed when it should have succeeded.");

        return ret.Response;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined active test permission records in the data store.
    /// </summary>
    public async Task<List<PermissionDto>> CreateActiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<PermissionDto>();
        var recordsToCreate = new List<InsertUpdatePermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSinglePermissionTestRecord(applicationId, true));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined inactive test permission records in the data store.
    /// </summary>
    public async Task<List<PermissionDto>> CreateInactiveTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<PermissionDto>();
        var recordsToCreate = new List<InsertUpdatePermissionRequest>();

        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            ret.Add(await CreateSinglePermissionTestRecord(applicationId, false));
        }

        return ret;
    }

    /// <summary>
    /// Asynchronously creates a set of predefined test active read-only permission records in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application to which the permissions belong.</param>
    /// <param name="numberOfRecordsToCreate">The number of active read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created active read-only permission DTOs.</returns>
    public async Task<List<PermissionDto>> CreateActiveReadOnlyTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        return await CreateReadOnlyTestRecords(applicationId, true, numberOfRecordsToCreate);
    }

    /// <summary>
    /// Asynchronously creates a set of predefined test inactive read-only permission records in the data store.
    /// </summary>
    /// <param name="applicationId">The ID of the application to which the permissions belong.</param>
    /// <param name="numberOfRecordsToCreate">The number of inactive read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created inactive read-only permission DTOs.</returns>
    public async Task<List<PermissionDto>> CreateInactiveReadOnlyTestRecords(int applicationId, short numberOfRecordsToCreate = 5)
    {
        return await CreateReadOnlyTestRecords(applicationId, false, numberOfRecordsToCreate);
    }

    /// <summary>
    /// Asynchronously deletes all records, including inactive ones, from the data store.
    /// </summary>
    public async Task DeleteAllRecords()
    {
        using var dbContext = _dbContextFactory.CreateContextReadWrite();
        await dbContext.Permissions.ExecuteDeleteAsync();
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
            { "Permission", new List<string> { "Record does not exist for specified PermissionId!" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedRequiredFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Name is a required field!" } },
            { "ApplicationId", new List<string> { "ApplicationId is a required field!" } },
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
    /// Retrieves a dictionary of expected read-only field validation error messages.
    /// </summary>
    /// <returns>A dictionary where the key is the read-only field name and the value is a list of error messages associated with that read-only field.</returns>
    public Dictionary<string, List<string>> GetExpectedReadOnlyErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "Permission", new List<string> { "Record is read only and cannot be modified! (IE: ReadOnly property is set to true)" } }
        };
    }

    public Dictionary<string, List<string>> GetExpectedInvalidApplicationIdFieldErrors()
    {
        return new Dictionary<string, List<string>>
        {
            { "ApplicationId", new List<string> { "Record does not exist for specified ApplicationId!" } }
        };
    }

    /// <summary>
    /// Verifies that all relevant property values of two permission records are equal.
    /// </summary>
    public void VerifyTestRecordValuesMatch(PermissionDto recordA, PermissionDto recordB)
    {
        recordA.PermissionId.Should().Be(recordB.PermissionId);
        recordA.Name.Should().Be(recordB.Name);
        recordA.Description.Should().Be(recordB.Description);
        recordA.Active.Should().Be(recordB.Active);
        recordA.ReadOnly.Should().Be(recordB.ReadOnly);
        recordA.ApplicationId.Should().Be(recordB.ApplicationId);
        recordA.CreatedBy.Should().Be(recordB.CreatedBy);
        recordA.UpdatedBy.Should().Be(recordB.UpdatedBy);
    }

    #region Private

    /// <summary>
    /// Asynchronously creates a set of predefined test read-only permission records in the data store.
    /// </summary>
    /// <param name="active">Indicates whether the created read-only test records should be active. Default is true.</param>
    /// <param name="numberOfRecordsToCreate">The number of read-only test records to create. Default is 5.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of created read-only permission DTOs.</returns>
    private async Task<List<PermissionDto>> CreateReadOnlyTestRecords(int applicationId, bool active = true, short numberOfRecordsToCreate = 5)
    {
        //create test records
        var ret = new List<PermissionDto>();
        
        for (var idx = 0; idx < numberOfRecordsToCreate; idx++)
        {
            var insertReq = CreateInsertUpdateRequestWithRandomValues(applicationId, active);
            var ent = insertReq.ToEntityOnInsert();
            ent.ReadOnly = true;

            using (var dbContext = _dbContextFactory.CreateContextReadWrite())
            {
                await dbContext.Permissions.AddAsync(ent);
                await dbContext.SaveChangesAsync();
                ret.Add(ent.ToDto());
            }
        }

        return ret;
    }

    #endregion 
}
