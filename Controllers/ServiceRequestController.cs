using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestService _srService;
        private readonly IContractService _contractService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestController(
            IServiceRequestService srService,
            IContractService contractService,
            ICurrencyService currencyService)
        {
            _srService = srService;
            _contractService = contractService;
            _currencyService = currencyService;
        }

        // LIST all service requests
        public async Task<IActionResult> Index()
        {
            var requests = await _srService.GetAllAsync();
            return View(requests);
        }

        // SHOW create form
        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropdown();
            return View();
        }

        // HANDLE create with currency conversion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request, decimal usdAmount)
        {
            if (ModelState.IsValid)
            {
                // Get live exchange rate
                var rate = await _currencyService.GetUsdToZarRateAsync();

                // Pass rate through CostZAR temporarily as the exchange rate
                request.CostZAR = rate;

                var (success, message) = await _srService.CreateRequestAsync(request, usdAmount);

                if (success)
                    return RedirectToAction(nameof(Index));

                // Workflow validation failed (Expired/On Hold contract)
                ModelState.AddModelError("", message);
            }

            await PopulateContractsDropdown();
            return View(request);
        }

        // SHOW edit form
        public async Task<IActionResult> Edit(int id)
        {
            var sr = await _srService.GetByIdAsync(id);
            if (sr == null) return NotFound();
            await PopulateContractsDropdown();
            return View(sr);
        }

        // HANDLE edit submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest request)
        {
            if (id != request.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                await _srService.UpdateAsync(request);
                return RedirectToAction(nameof(Index));
            }

            await PopulateContractsDropdown();
            return View(request);
        }

        // SHOW details
        public async Task<IActionResult> Details(int id)
        {
            var sr = await _srService.GetByIdAsync(id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        // SHOW delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var sr = await _srService.GetByIdAsync(id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        // HANDLE delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _srService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // API endpoint - get live rate for frontend auto-calculation
        [HttpGet]
        public async Task<IActionResult> GetRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Json(new { rate });
        }

        private async Task PopulateContractsDropdown()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            ViewBag.Contracts = new SelectList(contracts, "ContractId", "ServiceLevel");
        }
    }
}
