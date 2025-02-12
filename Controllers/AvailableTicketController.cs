using exam1.models;
using exam1.services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exam1.Controllers
{
    [Route("api/v1-get-available-ticket")]
    [ApiController]
    public class AvailableTicketController : ControllerBase
    {
        private readonly AvailableTicketService _services;
        private readonly ILogger<AvailableTicketController> _logger;
        public AvailableTicketController(AvailableTicketService services, ILogger<AvailableTicketController> logger)
        {
            _services = services;
            _logger = logger;
        }



        // GET: api/<AvailableTicketController>
        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] AvailableTicketRequestModel request)
        {
            _logger.LogInformation("Start of method test");

            var data = await _services.GetAvailableTicketData(request);

            return Ok(data);

        }
    }
}
