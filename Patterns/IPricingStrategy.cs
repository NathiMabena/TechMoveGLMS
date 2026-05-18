namespace TechMoveGLMS.Patterns
{

    public interface IPricingStrategy
    {
        decimal CalculateTotalCost(decimal usdAmount, decimal exchangeRate);
    }
}
