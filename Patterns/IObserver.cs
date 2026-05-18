namespace TechMoveGLMS.Patterns
{
    public interface IContractObserver
    {
        void Update(int contractId, string newStatus);
    }
}