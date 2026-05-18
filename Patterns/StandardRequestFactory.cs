using TechMoveGLMS.Models;



namespace TechMoveGLMS.Patterns
{
    public class StandardRequestFactory : IServiceRequestFactory
    {
        public ServiceRequest CreateRequest(int contractId, string description)
        {
            return new ServiceRequest
            {
                ContractId = contractId,
                Description = description,
                Status = "Pending",
                CostUSD = 0,
                CostZAR = 0
            };
        }
    }
}