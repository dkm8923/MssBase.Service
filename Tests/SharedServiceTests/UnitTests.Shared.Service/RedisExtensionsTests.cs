using System.Net;
using System.Text.Json;
using Moq;
using Shared.Service.Cache.Redis;
using Shared.Models;
using StackExchange.Redis;
using Xunit;

namespace UnitTests.EpcService.Shared;

public class RedisExtensionsTests
{
    private record TestClass;

    private readonly RedisExtensions _redisExtensions;
    private readonly Mock<IDatabase> _iDatabaseMock;
    private readonly Mock<IConnectionMultiplexer> _iConnectionMultiplexerMock;

    public RedisExtensionsTests()
    {
        _iConnectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
        _iDatabaseMock = new Mock<IDatabase>();
        _iConnectionMultiplexerMock.Setup(t => t.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_iDatabaseMock.Object);
        _redisExtensions = new RedisExtensions(_iConnectionMultiplexerMock.Object);
    }

    [Fact]
    public async Task GetByKeyAsync_Exists_Test()
    {
        // Arrange
        var deleteCache = false;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue(value));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Errors = new Dictionary<string, List<string>>
                {
                    { "", [""] }
                }
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(result.Response is TestClass);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GetByKeyAsync_Exists_DeleteCache_Test()
    {
        // Arrange
        var deleteCache = true;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue(value));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Response = new TestClass()
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(result.Response is TestClass);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GetByKeyAsync_Exists_Error_DeleteCache_Test()
    {
        // Arrange
        var deleteCache = true;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue(null));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Errors = new Dictionary<string, List<string>>
                {
                    { "", [""] }
                }
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Null(result.Response);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task GetByKeyAsync_Missing_Test()
    {
        // Arrange
        var deleteCache = false;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue(value));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Errors = new Dictionary<string, List<string>>
                {
                    { "", [""] }
                }
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(result.Response is TestClass);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GetByKeyAsync_Missing_DeleteCache_Test()
    {
        // Arrange
        var deleteCache = true;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue(value));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Response = new TestClass()
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(result.Response is TestClass);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task RemoveKeysByPatternAsync_Test()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1));
        _iDatabaseMock.Setup(t => t.Multiplexer.GetEndPoints(It.IsAny<bool>())).Returns([new IPEndPoint(1, 1)]);
        var serverMock = new Mock<IServer>();
        var scanResult = new RedisResult[] { RedisResult.Create(0, ResultType.Integer), RedisResult.Create(1, ResultType.Integer) };
        serverMock.Setup(t => t.ExecuteAsync(It.IsAny<string>(), It.IsAny<object[]>())).ReturnsAsync(RedisResult.Create(scanResult));
        _iDatabaseMock.Setup(t => t.Multiplexer.GetServer(It.IsAny<EndPoint>(), It.IsAny<object?>())).Returns(serverMock.Object);

        // Act
        var task = _redisExtensions.RemoveKeysByPatternAsync(key);
        await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        _iDatabaseMock.Verify(t => t.KeyDeleteAsync(new RedisKey("1"), It.IsAny<CommandFlags>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByKeyAsync_NoConnection_Response_Test()
    {
        // Arrange
        var deleteCache = false;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1001));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Response = new TestClass()
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.True(result.Response is TestClass);
        Assert.NotNull(result.Response);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public async Task GetByKeyAsync_NoConnection_Error_Test()
    {
        // Arrange
        var deleteCache = false;
        var key = Guid.NewGuid().ToString();
        var value = JsonSerializer.Serialize(new TestClass());
        _iDatabaseMock.Setup(t => t.Ping(It.IsAny<CommandFlags>())).Returns(TimeSpan.FromMilliseconds(1001));
        var fun = () =>
        {
            return Task.FromResult(new ErrorValidationResult<TestClass>
            {
                Errors = new Dictionary<string, List<string>>
                {
                    { "", [""] }
                }
            });
        };

        // Act
        var task = _redisExtensions.GetByKeyAsync(deleteCache, key, fun);
        var result = await task;

        // Assert
        Assert.True(task.IsCompletedSuccessfully);
        Assert.Null(result.Response);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
    }
}