using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> GetAllContractsAsync();
        Task<IEnumerable<Contract>> SearchContractsAsync(string? status, DateTime? startDate, DateTime? endDate);
        Task<Contract?> GetContractByIdAsync(int id);
        Task AddContractAsync(Contract contract, IFormFile? file);
        Task UpdateContractAsync(Contract contract);
        Task DeleteContractAsync(int id);
    }
}