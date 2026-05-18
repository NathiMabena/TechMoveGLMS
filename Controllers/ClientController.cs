using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IContractService _contractService;

        public ClientController(IClientService clientService, IContractService contractService)
        {
            _clientService = clientService;
            _contractService = contractService;
        }

        // LIST all clients
        public async Task<IActionResult> Index()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return View(clients);
        }

        // SHOW create form
        public IActionResult Create()
        {
            return View();
        }

        // HANDLE create form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                await _clientService.AddClientAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // SHOW edit form
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        // HANDLE edit form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                await _clientService.UpdateClientAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // SHOW delete confirmation with warning
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) return NotFound();

            var contracts = await _contractService.GetAllContractsAsync();
            var clientContracts = contracts.Where(c => c.ClientId == id).ToList();
            ViewBag.ContractCount = clientContracts.Count;
            ViewBag.Contracts = clientContracts;

            return View(client);
        }

        // HANDLE delete confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientService.DeleteClientAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // SHOW details
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }
    }
}