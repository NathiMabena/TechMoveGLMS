namespace TechMoveGLMS.Patterns
{
    public class ServiceRequestValidator : IContractObserver
    {
        private readonly List<int> _blockedContractIds = new();

        public void Update(int contractId, string newStatus)
        {
            if (newStatus == "Expired" || newStatus == "On Hold")
            {
                if (!_blockedContractIds.Contains(contractId))
                    _blockedContractIds.Add(contractId);
            }
            else
            {
                _blockedContractIds.Remove(contractId);
            }
        }

        public bool IsContractBlocked(int contractId)
        {
            return _blockedContractIds.Contains(contractId);
        }
    }
}