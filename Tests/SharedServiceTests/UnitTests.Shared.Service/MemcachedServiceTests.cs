using System.Text.Json;
using Enyim.Caching.Memcached;
using Moq;
using Shared.Service.Cache.Memcached;
using Shared.Models;
using Xunit;

namespace UnitTests.EpcService.Shared;

public class MemcachedServiceTests
{
	private record TestClass
	{
		public Guid Id { get; set; } = Guid.NewGuid();
	}

	private readonly Mock<IMemcachedClient> _iMemcachedClientMock;
	private readonly MemcachedService _memoryCacheService;

	public MemcachedServiceTests()
	{
		_iMemcachedClientMock = new Mock<IMemcachedClient>();
		_memoryCacheService = new MemcachedService(_iMemcachedClientMock.Object);
	}

	[Fact]
	public async Task GetByKeyAsync_Exists_Test()
	{
		// Arrange
		var deleteCache = false;
		var valueFromCache = new TestClass();
		var valueFromFunction = new TestClass();
		var keyName = Guid.NewGuid().ToString();
		_iMemcachedClientMock.Verify(t => t.DeleteWithResultAsync(It.IsAny<string>(), It.IsAny<ulong>()), Times.Never);
		_iMemcachedClientMock.Setup(t => t.GetWithResultAsync<string>(It.IsAny<string>(), It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult<string>(JsonSerializer.Serialize(valueFromCache), OperationStatus.Success, 0));
		var fun = () =>
		{
			return Task.FromResult(new ErrorValidationResult<TestClass>
			{
				Response = valueFromFunction
			});
		};

		// Act
		var task = _memoryCacheService.GetByKeyAsync(deleteCache, keyName, fun);
		var result = await task;

		// Assert
		Assert.True(task.IsCompletedSuccessfully);
		Assert.Empty(result.Errors);
		Assert.Equal(valueFromCache.Id, result.Response.Id);
	}

	[Fact]
	public async Task RemoveKeysByPatternAsync_Exists_Test()
	{
		// Arrange
		var pattern = Guid.NewGuid().ToString();
		string[] arr = [pattern];
		_iMemcachedClientMock.Setup(t => t.GetWithResultAsync<string>("ALL_KEYS", It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult<string>(JsonSerializer.Serialize(arr), OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.DeleteWithResultAsync(pattern, It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.StoreWithResultAsync(StoreMode.Set, "ALL_KEYS", "[]", It.IsAny<ulong>(), Expiration.Never))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Once);

		// Act
		var task = _memoryCacheService.RemoveKeysByPatternAsync(pattern);
		await task;

		// Assert
		Assert.True(task.IsCompletedSuccessfully);
		Mock.Verify(_iMemcachedClientMock);
	}

	[Fact]
	public async Task RemoveKeysByPatternAsync_Exists_Multiple_Test()
	{
		// Arrange
		var pattern = Guid.NewGuid().ToString();
		var key1 = $"{pattern}-1";
		var key2 = $"{pattern}-2";
		var extra = "test_abc_123";
		var expectedKey = JsonSerializer.Serialize<IEnumerable<string>>([extra]);
		_iMemcachedClientMock.Setup(t => t.GetWithResultAsync<string>("ALL_KEYS", It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult<string>(JsonSerializer.Serialize<IEnumerable<string>>([key1, key2, extra]), OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.DeleteWithResultAsync(key1, It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.DeleteWithResultAsync(key2, It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.StoreWithResultAsync(StoreMode.Set, "ALL_KEYS", expectedKey, It.IsAny<ulong>(), Expiration.Never))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Once);

		// Act
		var task = _memoryCacheService.RemoveKeysByPatternAsync(pattern);
		await task;

		// Assert
		Assert.True(task.IsCompletedSuccessfully);
		Mock.Verify(_iMemcachedClientMock);
	}

	[Fact]
	public async Task RemoveKeysByPatternAsync_Missing_Test()
	{
		// Arrange
		var pattern = Guid.NewGuid().ToString();
		_iMemcachedClientMock.Setup(t => t.GetWithResultAsync<string>("ALL_KEYS", It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult<string>(JsonSerializer.Serialize(new List<string>()), OperationStatus.Success, 0))
			.Verifiable(Times.Once);
		_iMemcachedClientMock.Setup(t => t.DeleteWithResultAsync(It.IsAny<string>(), It.IsAny<ulong>()))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Never);
		_iMemcachedClientMock.Setup(t => t.StoreWithResultAsync(It.IsAny<StoreMode>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ulong>(), It.IsAny<Expiration>()))
			.ReturnsAsync(new OperationResult(OperationStatus.Success, 0))
			.Verifiable(Times.Never);

		// Act
		var task = _memoryCacheService.RemoveKeysByPatternAsync(pattern);
		await task;

		// Assert
		Assert.True(task.IsCompletedSuccessfully);
		Mock.Verify(_iMemcachedClientMock);
	}
}