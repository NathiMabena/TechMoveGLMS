using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        // GET: api/servicerequest
        [HttpGet]
        public async Task<IActionResult> GetAllServiceRequests()
        {
            var requests = await _serviceRequestService.GetAllAsync();
            return Ok(requests);
        }

        // GET: api/servicerequest/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound($"Service Request with ID {id} not found.");
            return Ok(request);
        }

        // POST: api/servicerequest
        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _serviceRequestService.CreateRequestAsync(request,
                    request.CostUSD);
                if (!result.success) return BadRequest(result.message);
                return CreatedAtAction(nameof(GetServiceRequest),
                    new { id = request.ServiceRequestId }, request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/servicerequest/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id,
            [FromBody] ServiceRequest request)
        {
            if (id != request.ServiceRequestId) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _serviceRequestService.UpdateAsync(request);
            return NoContent();
        }

        // PATCH: api/servicerequest/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id,
            [FromBody] string newStatus)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound($"Service Request with ID {id} not found.");
            request.Status = newStatus;
            await _serviceRequestService.UpdateAsync(request);
            return NoContent();
        }

        // DELETE: api/servicerequest/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound();
            await _serviceRequestService.DeleteAsync(id);
            return NoContent();
        }
    }
}