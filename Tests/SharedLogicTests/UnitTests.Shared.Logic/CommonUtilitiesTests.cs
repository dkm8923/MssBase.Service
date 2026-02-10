using Shared.Logic.Common;
using FluentAssertions;
using System.Net;

namespace UnitTests.EpcService.Shared
{
    public class CommonUtilitiesTests
    {
        #region GetDateTimeUtcNow

        [Fact]
        public void GetDateTimeUtcNow_Returns_DateTime_UTCNow()
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Act
            var result = CommonUtilities.GetDateTimeUtcNow();

            // Assert
            result.Date.Should().Be(now.Date);
            result.Hour.Should().Be(now.Hour);
            result.Minute.Should().Be(now.Minute);
        }

        #endregion

        #region IsGenericTypeCollectionWithData

        [Fact]
        public void IsGenericTypeCollectionWithData_EmptyList_Should_Return_True()
        {
            // Arrange
            var req = new List<string>();

            // Act
            var result = CommonUtilities.IsGenericTypeCollectionWithData(req);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void IsGenericTypeCollectionWithData_PopulatedList_Should_Return_False()
        {
            // Arrange
            var req = new List<string> { "Test1", "Test2", "Test3" };

            // Act
            var result = CommonUtilities.IsGenericTypeCollectionWithData(req);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void IsGenericTypeCollectionWithData_DynamicObject_Should_Return_False()
        {
            // Arrange
            var req = new { Test1 = "Test1", Test2 = 1 };

            // Act
            var result = CommonUtilities.IsGenericTypeCollectionWithData(req);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void IsGenericTypeCollectionWithData_String_Should_Return_False()
        {
            // Arrange
            var req = "Test";

            // Act
            var result = CommonUtilities.IsGenericTypeCollectionWithData(req);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void IsGenericTypeCollectionWithData_NullString_Should_Return_False()
        {
            // Arrange
            string req = null;

            // Act
            var result = CommonUtilities.IsGenericTypeCollectionWithData(req);

            // Assert
            result.Should().Be(false);
        }

        #endregion

        #region RemoveWhiteSpaceFromString

        [Fact]
        public void RemoveWhiteSpaceFromString_Should_Remove_WhiteSpace()
        {
            // Arrange
            var req = "This Is A Test";
            var expectedResult = "ThisIsATest";

            // Act
            var result = CommonUtilities.RemoveWhiteSpaceFromString(req);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void RemoveWhiteSpaceFromString_Should_Not_Remove_WhiteSpace()
        {
            // Arrange
            var req = "ThisIsATest";
            var expectedResult = "ThisIsATest";

            // Act
            var result = CommonUtilities.RemoveWhiteSpaceFromString(req);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void RemoveWhiteSpaceFromString_Should_Return_Null()
        {
            // Arrange
            
            // Act
            var result = CommonUtilities.RemoveWhiteSpaceFromString(null);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RemoveWhiteSpaceFromString_Should_Return_Blank()
        {
            // Arrange
            var req = "";
            var expectedResult = "";

            // Act
            var result = CommonUtilities.RemoveWhiteSpaceFromString(req);

            // Assert
            result.Should().Be(expectedResult);
        }

        #endregion

        #region GetLocalIpAddress

        [Fact]
        public void GetLocalIpAddress_Should_Return_LocalIpAddress()
        {
            // Arrange
            
            // Act
            var result = CommonUtilities.GetLocalIpAddress();
            var isIpAddress = IPAddress.TryParse(result, out _);

            // Assert
            isIpAddress.Should().BeTrue();
        }

        #endregion

        #region IsBase64String

        [Fact]
        public void IsBase64String_Valid_Base64_String_Should_Return_True()
        {
            // Arrange
            var testStringVal = "r3k2ergD9p0VtW4Q7RJfBgmV+IH2xzmcZuWJ1UjDn5ghoo2u8DK9J7uTzJOYbEtSvPUccmvQ7ArbO7NGt/t106HL5APN3f+wATpGE/3oPEEtiLFJA6TU2ITn7NHYnyguC3pbb0Z3wRHXx6MPtQBqU5jICzje3OA7LpTv4lEMkwZJMSGQgs6qPRtPd62zTpX/jqq7Ia3lgJynkql8VUXnD3jtOKsHO9aFTh6kBlJiwMA=";

            // Act
            var result = CommonUtilities.IsBase64String(testStringVal);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBase64String_Invalid_Base64_String_Should_Return_False()
        {
            // Arrange
            var testStringVal = "ThisIsATest";

            // Act
            var result = CommonUtilities.IsBase64String(testStringVal);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBase64String_Blank_Value_String_Should_Return_False()
        {
            // Arrange
            var testStringVal = "";

            // Act
            var result = CommonUtilities.IsBase64String(testStringVal);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBase64String_Null_Value_String_Should_Return_False()
        {
            // Arrange
            
            // Act
            var result = CommonUtilities.IsBase64String(null);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
