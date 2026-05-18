using Microsoft.EntityFrameworkCore;
using System;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Patterns;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPricingStrategy _pricingStrategy;
        private readonly IServiceRequestFactory _factory;

        public ServiceRequestService(
            ApplicationDbContext context,
            IPricingStrategy pricingStrategy,
            IServiceRequestFactory factory)
        {
            _context = context;
            _pricingStrategy = pricingStrategy;
            _factory = factory;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
            => await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ToListAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id)
            => await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);

        public async Task<(bool success, string message)> CreateRequestAsync(
            ServiceRequest request, decimal usdAmount)
        {
            // Observer/Workflow logic - block Expired or On Hold contracts
            var contract = await _context.Contracts.FindAsync(request.ContractId);

            if (contract == null)
                return (false, "Contract not found.");

            if (contract.Status == "Expired" || contract.Status == "On Hold")
                return (false, $"Cannot create a Service Request for a contract that is '{contract.Status}'.");

            // Use factory to build the request object
            var newRequest = _factory.CreateRequest(request.ContractId, request.Description);

            // Use strategy pattern for cost calculation
            newRequest.CostUSD = usdAmount;
            newRequest.CostZAR = _pricingStrategy.CalculateTotalCost(usdAmount, request.CostZAR);
            newRequest.Status = request.Status;

            _context.ServiceRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return (true, "Service Request created successfully.");
        }

        public async Task UpdateAsync(ServiceRequest request)
        {
            _context.ServiceRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sr = await _context.ServiceRequests.FindAsync(id);
            if (sr != null)
            {
                _context.ServiceRequests.Remove(sr);
                await _context.SaveChangesAsync();
            }
        }
    }
}