using TechMoveGLMS.Patterns;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class FactoryPatternTests
    {
        [Fact]
        public void StandardFactory_CreatesRequest_WithCorrectContractId()
        {
            // Arrange
            var factory = new StandardRequestFactory();

            // Act
            var request = factory.CreateRequest(1, "Test Description");

            // Assert
            Assert.Equal(1, request.ContractId);
        }

        [Fact]
        public void StandardFactory_CreatesRequest_WithCorrectDescription()
        {
            // Arrange
            var factory = new StandardRequestFactory();

            // Act
            var request = factory.CreateRequest(1, "Freight to Johannesburg");

            // Assert
            Assert.Equal("Freight to Johannesburg", request.Description);
        }

        [Fact]
        public void StandardFactory_CreatesRequest_WithPendingStatus()
        {
            // Arrange
            var factory = new StandardRequestFactory();

            // Act
            var request = factory.CreateRequest(1, "Test");

            // Assert
            Assert.Equal("Pending", request.Status);
        }

        [Fact]
        public void StandardFactory_CreatesRequest_WithZeroCost()
        {
            // Arrange
            var factory = new StandardRequestFactory();

            // Act
            var request = factory.CreateRequest(1, "Test");

            // Assert
            Assert.Equal(0, request.CostUSD);
            Assert.Equal(0, request.CostZAR);
        }

        [Fact]
        public void StandardFactory_CreatesRequest_NotNull()
        {
            // Arrange
            var factory = new StandardRequestFactory();

            // Act
            var request = factory.CreateRequest(1, "Test");

            // Assert
            Assert.NotNull(request);
        }
    }
}