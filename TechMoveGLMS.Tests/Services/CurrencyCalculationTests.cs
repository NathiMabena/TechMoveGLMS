using TechMoveGLMS.Patterns;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class CurrencyCalculationTests
    {
        private readonly StandardFreightPricing _standardPricing;
        private readonly ExpeditedAirPricing _expeditedPricing;

        public CurrencyCalculationTests()
        {
            _standardPricing = new StandardFreightPricing();
            _expeditedPricing = new ExpeditedAirPricing();
        }

        // Standard pricing tests
        [Fact]
        public void StandardPricing_CorrectConversion_ReturnsExpectedZAR()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = _standardPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void StandardPricing_ZeroAmount_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 0;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = _standardPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void StandardPricing_ZeroRate_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal exchangeRate = 0;

            // Act
            decimal result = _standardPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void StandardPricing_LargeAmount_CalculatesCorrectly()
        {
            // Arrange
            decimal usdAmount = 50000;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = _standardPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(925000.00m, result);
        }

        // Expedited pricing tests
        [Fact]
        public void ExpeditedPricing_AppliesTwentyPercentSurcharge()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = _expeditedPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert — should be 100 * 18.50 * 1.20 = 2220
            Assert.Equal(2220.00m, result);
        }

        [Fact]
        public void ExpeditedPricing_ZeroAmount_ReturnsZero()
        {
            // Arrange
            decimal usdAmount = 0;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = _expeditedPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ExpeditedPricing_IsMoreExpensiveThanStandard()
        {
            // Arrange
            decimal usdAmount = 100;
            decimal exchangeRate = 18.50m;

            // Act
            decimal standard = _standardPricing.CalculateTotalCost(usdAmount, exchangeRate);
            decimal expedited = _expeditedPricing.CalculateTotalCost(usdAmount, exchangeRate);

            // Assert
            Assert.True(expedited > standard);
        }
    }
}