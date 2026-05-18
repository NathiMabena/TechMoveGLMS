using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IContractService _contractService;
        private readonly IServiceRequestService _srService;

        public HomeController(
            IClientService clientService,
            IContractService contractService,
            IServiceRequestService srService)
        {
            _clientService = clientService;
            _contractService = contractService;
            _srService = srService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientService.GetAllClientsAsync();
            var contracts = await _contractService.GetAllContractsAsync();
            var requests = await _srService.GetAllAsync();

            ViewBag.TotalClients = clients.Count();
            ViewBag.ActiveContracts = contracts.Count(c => c.Status == "Active");
            ViewBag.TotalRequests = requests.Count();

            return View();
        }
    }
}