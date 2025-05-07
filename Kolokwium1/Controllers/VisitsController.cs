using Kolokwium1.Models.DTOs;
using Kolokwium1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public VisitsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisit(int id)
        {
            if(!await _dbService.DoesVisitExist(id))
            {
                return NotFound();
            }
            var visit = await _dbService.GetVisit(id);
            return Ok(visit);
        }
        [HttpPost]
        public async Task<IActionResult> AddVisit([FromBody] AddVisitDTO visit)
        {
            if (!await _dbService.DoesClientExist(visit.clientId))
            {
                return NotFound($"Client with ID {visit.clientId} does not exist.");
            }
            if (!await _dbService.DoesMechanicExist(visit.mechanicLicenceNumber))
            {
                return NotFound($"Mechanic with licence number {visit.mechanicLicenceNumber} does not exist.");
            }
            foreach (var service in visit.services)
            {
                if (!await _dbService.DoesServiceExist(service.serviceName))
                {
                    return NotFound($"Service with name {service.serviceName} does not exist.");
                }
            }
            if (visit == null)
            {
                return BadRequest("Invalid visit data.");
            }
            var visitId = await _dbService.AddVisit(visit);
            return CreatedAtAction(nameof(GetVisit), new { id = visitId }, visit);
        }
    }
}
