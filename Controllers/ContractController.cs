using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class ContractController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContractController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Contract
        public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            string requestUrl = "api/contract";
            if (!string.IsNullOrEmpty(status))
            {
                requestUrl += $"?statusFilter={status}";
            }

            var response = await client.GetAsync(requestUrl);
            IEnumerable<Contract> contracts = new List<Contract>();

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                contracts = JsonConvert.DeserializeObject<IEnumerable<Contract>>(jsonString) ?? new List<Contract>();
            }

            ViewBag.SelectedStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Statuses = new SelectList(new[] { "Draft", "Active", "Expired", "On Hold" });

            return View(contracts);
        }

        // GET: /Contract/Create
        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropdown();
            return View();
        }

        // POST: /Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contractModel, IFormFile? signedAgreement)
        {
            ModelState.Remove("signedAgreement");

            if (ModelState.IsValid)
            {
                // Handle PDF upload locally
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    var extension = Path.GetExtension(signedAgreement.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("", "Only PDF files are allowed.");
                        await PopulateClientsDropdown();
                        return View(contractModel);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + ".pdf";
                    var uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "uploads", "agreements");
                    Directory.CreateDirectory(uploadFolder);
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await signedAgreement.CopyToAsync(stream);
                    }

                    contractModel.SignedAgreementPath = $"/uploads/agreements/{uniqueFileName}";
                }

                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                var json = JsonConvert.SerializeObject(contractModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/contract", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Failed to create contract via API.");
            }

            await PopulateClientsDropdown();
            return View(contractModel);
        }

        // GET: /Contract/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/contract/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var contractModel = JsonConvert.DeserializeObject<Contract>(jsonString);

            await PopulateClientsDropdown();
            return View(contractModel);
        }

        // POST: /Contract/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contractModel, IFormFile? signedAgreement)
        {
            if (id != contractModel.ContractId) return NotFound();
            ModelState.Remove("signedAgreement");

            if (ModelState.IsValid)
            {
                // Handle new PDF if uploaded locally
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    var extension = Path.GetExtension(signedAgreement.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        ModelState.AddModelError("", "Only PDF files are allowed.");
                        await PopulateClientsDropdown();
                        return View(contractModel);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + ".pdf";
                    var uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "uploads", "agreements");
                    Directory.CreateDirectory(uploadFolder);
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await signedAgreement.CopyToAsync(stream);
                    }

                    contractModel.SignedAgreementPath = $"/uploads/agreements/{uniqueFileName}";
                }

                var client = _httpClientFactory.CreateClient("GlmsApi");
                AddJwtHeader(client);

                // Use PUT to update all contract fields
                var json = JsonConvert.SerializeObject(contractModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/contract/{id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Failed to update contract via API.");
            }

            await PopulateClientsDropdown();
            return View(contractModel);
        }

        // GET: /Contract/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/contract/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var contractModel = JsonConvert.DeserializeObject<Contract>(jsonString);

            return View(contractModel);
        }

        // POST: /Contract/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            // Ensure your API actually has a DELETE endpoint!
            await client.DeleteAsync($"api/contract/{id}");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Contract/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync($"api/contract/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var contractModel = JsonConvert.DeserializeObject<Contract>(jsonString);

            return View(contractModel);
        }

        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/pdf", Path.GetFileName(fullPath));
        }

        private async Task PopulateClientsDropdown()
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            AddJwtHeader(client);

            var response = await client.GetAsync("api/client");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(jsonString) ?? new List<Client>();
                ViewBag.Clients = new SelectList(clients, "ClientId", "Name");
            }
        }

        private void AddJwtHeader(HttpClient client)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}