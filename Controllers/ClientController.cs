using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class ClientController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Client
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync("api/client");
            IEnumerable<Client> clients = new List<Client>();

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(jsonString)
                    ?? new List<Client>();
            }

            return View(clients);
        }

        // GET: /Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client clientModel)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                var json = JsonConvert.SerializeObject(clientModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/client", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Failed to create client via API.");
            }

            return View(clientModel);
        }

        // GET: /Client/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/client/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var clientModel = JsonConvert.DeserializeObject<Client>(jsonString);

            return View(clientModel);
        }

        // POST: /Client/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client clientModel)
        {
            if (id != clientModel.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                var json = JsonConvert.SerializeObject(clientModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/client/{id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Failed to update client via API.");
            }

            return View(clientModel);
        }

        // GET: /Client/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/client/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var clientModel = JsonConvert.DeserializeObject<Client>(jsonString);

            // Get contracts for warning
            var contractsResponse = await client.GetAsync("api/contract");
            if (contractsResponse.IsSuccessStatusCode)
            {
                var contractsJson = await contractsResponse.Content.ReadAsStringAsync();
                var allContracts = JsonConvert.DeserializeObject<IEnumerable<Contract>>(
                    contractsJson) ?? new List<Contract>();
                var clientContracts = allContracts
                    .Where(c => c.ClientId == id).ToList();
                ViewBag.ContractCount = clientContracts.Count;
                ViewBag.Contracts = clientContracts;
            }

            return View(clientModel);
        }

        // POST: /Client/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            await client.DeleteAsync($"api/client/{id}");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Client/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/client/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var clientModel = JsonConvert.DeserializeObject<Client>(jsonString);

            return View(clientModel);
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
    }
}