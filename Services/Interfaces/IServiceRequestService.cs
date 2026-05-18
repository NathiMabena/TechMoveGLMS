using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services.Interfaces
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<(bool success, string message)> CreateRequestAsync(ServiceRequest request, decimal usdAmount);
        Task UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}