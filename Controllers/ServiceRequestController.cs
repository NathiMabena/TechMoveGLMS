using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestController(
            IHttpClientFactory httpClientFactory,
            ICurrencyService currencyService)
        {
            _httpClientFactory = httpClientFactory;
            _currencyService = currencyService;
        }

        // GET: /ServiceRequest
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync("api/servicerequest");
            IEnumerable<ServiceRequest> requests = new List<ServiceRequest>();

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                requests = JsonConvert.DeserializeObject<IEnumerable<ServiceRequest>>(
                    jsonString) ?? new List<ServiceRequest>();
            }

            return View(requests);
        }

        // GET: /ServiceRequest/Create
        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropdown();
            return View();
        }

        // POST: /ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request, decimal usdAmount)
        {
            if (ModelState.IsValid)
            {
                // Get live exchange rate
                var rate = await _currencyService.GetUsdToZarRateAsync();
                request.CostUSD = usdAmount;
                request.CostZAR = usdAmount * rate;

                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/servicerequest", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                // Read error message from API
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMessage);
            }

            await PopulateContractsDropdown();
            return View(request);
        }

        // GET: /ServiceRequest/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/servicerequest/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var sr = JsonConvert.DeserializeObject<ServiceRequest>(jsonString);

            await PopulateContractsDropdown();
            return View(sr);
        }

        // POST: /ServiceRequest/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest request,
            decimal usdAmount)
        {
            if (id != request.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                // Recalculate cost if USD amount changed
                if (usdAmount > 0)
                {
                    var rate = await _currencyService.GetUsdToZarRateAsync();
                    request.CostUSD = usdAmount;
                    request.CostZAR = usdAmount * rate;
                }

                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/servicerequest/{id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Failed to update request via API.");
            }

            await PopulateContractsDropdown();
            return View(request);
        }

        // GET: /ServiceRequest/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/servicerequest/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var sr = JsonConvert.DeserializeObject<ServiceRequest>(jsonString);

            return View(sr);
        }

        // GET: /ServiceRequest/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/servicerequest/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var sr = JsonConvert.DeserializeObject<ServiceRequest>(jsonString);

            return View(sr);
        }

        // POST: /ServiceRequest/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            await client.DeleteAsync($"api/servicerequest/{id}");
            return RedirectToAction(nameof(Index));
        }

        // API endpoint - get live rate for frontend
        [HttpGet]
        public async Task<IActionResult> GetRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Json(new { rate });
        }

        // Helper - adds JWT token to every API request
        private void AddJwtHeader(HttpClient client)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);
        }

        // Helper - populates contracts dropdown via API
        private async Task PopulateContractsDropdown()
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync("api/contract");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var contracts = JsonConvert.DeserializeObject<IEnumerable<Contract>>(
                    jsonString) ?? new List<Contract>();
                ViewBag.Contracts = new SelectList(contracts, "ContractId", "ServiceLevel");
            }
        }
    }
}