using TechMoveGLMS.Models;

namespace TechMoveGLMS.Patterns
{
    public interface IServiceRequestFactory
    {
        ServiceRequest CreateRequest(int contractId, string description);
    }
}