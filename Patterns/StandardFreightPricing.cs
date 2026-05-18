namespace TechMoveGLMS.Patterns
{
    public class StandardFreightPricing : IPricingStrategy
    {
        public decimal CalculateTotalCost(decimal usdAmount, decimal exchangeRate)
        {
            // Standard: straight conversion
            return usdAmount * exchangeRate;
        }
    }
}