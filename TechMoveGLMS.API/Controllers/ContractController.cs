using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        // GET: api/contract
        [HttpGet]
        public async Task<IActionResult> GetAllContracts([FromQuery] string? statusFilter)
        {
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var filtered = await _contractService.SearchContractsAsync(
                    statusFilter, null, null);
                return Ok(filtered);
            }
            var contracts = await _contractService.GetAllContractsAsync();
            return Ok(contracts);
        }

        // GET: api/contract/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        // POST: api/contract
        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] Contract contract)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Server side date validation
            if (contract.EndDate <= contract.StartDate)
                return BadRequest("End date must be after start date.");

            await _contractService.AddContractAsync(contract, null);
            return CreatedAtAction(nameof(GetContract),
                new { id = contract.ContractId }, contract);
        }

        // PUT: api/contract/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] Contract contract)
        {
            if (id != contract.ContractId) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Server side date validation
            if (contract.EndDate <= contract.StartDate)
                return BadRequest("End date must be after start date.");

            await _contractService.UpdateContractAsync(contract);
            return NoContent();
        }

        // PATCH: api/contract/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id,
            [FromBody] string newStatus)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound($"Contract with ID {id} not found.");
            contract.Status = newStatus;
            await _contractService.UpdateContractAsync(contract);
            return NoContent();
        }

        // DELETE: api/contract/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            await _contractService.DeleteContractAsync(id);
            return NoContent();
        }
    }
}