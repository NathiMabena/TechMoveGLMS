namespace TechMoveGLMS.Patterns
{
    public class ExpeditedAirPricing : IPricingStrategy
    {
        public decimal CalculateTotalCost(decimal usdAmount, decimal exchangeRate)
        {
            // Expedited: 20% surcharge on top of conversion
            return usdAmount * exchangeRate * 1.20m;
        }
    }
}