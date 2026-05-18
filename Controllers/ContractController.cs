using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientService _clientService;

        public ContractController(IContractService contractService, IClientService clientService)
        {
            _contractService = contractService;
            _clientService = clientService;
        }

        // LIST all contracts with search/filter
        public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate)
        {
            var contracts = await _contractService.SearchContractsAsync(status, startDate, endDate);

            // Pass filter values back to view
            ViewBag.SelectedStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Statuses = new SelectList(new[]
            {
                "Draft", "Active", "Expired", "On Hold"
            });

            return View(contracts);
        }

        // SHOW create form
        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropdown();
            return View();
        }

        // HANDLE create form submission with PDF upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _contractService.AddContractAsync(contract, signedAgreement);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    // This catches our "PDF only" validation error
                    ModelState.AddModelError("", ex.Message);
                }
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        // SHOW edit form
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            await PopulateClientsDropdown();
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.ContractId) return NotFound();

            if (ModelState.IsValid)
            {
                // If new PDF uploaded, process it
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    try
                    {
                        await _contractService.AddContractAsync(contract, signedAgreement);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        await PopulateClientsDropdown();
                        return View(contract);
                    }
                }
                else
                {
                    await _contractService.UpdateContractAsync(contract);
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        // SHOW delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // HANDLE delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contractService.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // SHOW details
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // DOWNLOAD signed agreement PDF
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/pdf", Path.GetFileName(fullPath));
        }

        // Helper to populate clients dropdown
        private async Task PopulateClientsDropdown()
        {
            var clients = await _clientService.GetAllClientsAsync();
            ViewBag.Clients = new SelectList(clients, "ClientId", "Name");
        }
    }
}