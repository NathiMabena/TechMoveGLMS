using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("GlmsApi");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);

            try
            {
                var clientsResponse = await client.GetAsync("api/client");
                var contractsResponse = await client.GetAsync("api/contract");
                var requestsResponse = await client.GetAsync("api/servicerequest");

                int totalClients = 0;
                int activeContracts = 0;
                int totalRequests = 0;

                if (clientsResponse.IsSuccessStatusCode)
                {
                    var json = await clientsResponse.Content.ReadAsStringAsync();
                    var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(json);
                    totalClients = clients?.Count() ?? 0;
                }

                if (contractsResponse.IsSuccessStatusCode)
                {
                    var json = await contractsResponse.Content.ReadAsStringAsync();
                    var contracts = JsonConvert.DeserializeObject<IEnumerable<Contract>>(json);
                    activeContracts = contracts?.Count(c => c.Status == "Active") ?? 0;
                }

                if (requestsResponse.IsSuccessStatusCode)
                {
                    var json = await requestsResponse.Content.ReadAsStringAsync();
                    var requests = JsonConvert.DeserializeObject<IEnumerable<ServiceRequest>>(json);
                    totalRequests = requests?.Count() ?? 0;
                }

                ViewBag.TotalClients = totalClients;
                ViewBag.ActiveContracts = activeContracts;
                ViewBag.TotalRequests = totalRequests;
            }
            catch
            {
                ViewBag.TotalClients = 0;
                ViewBag.ActiveContracts = 0;
                ViewBag.TotalRequests = 0;
            }

            return View();
        }
    }
}