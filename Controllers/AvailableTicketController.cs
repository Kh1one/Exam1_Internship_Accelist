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
        public AvailableTicketController(AvailableTicketService services)
        {
            _services = services;
        }

        // GET: api/<AvailableTicketController>
        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] AvailableTicketRequestModel request)
        {
            var data = await _services.GetAvailableTicketData(request);

            if (data.Status != 0)
            {
                data.Instance = HttpContext.Request.Path;
                return Ok(data);
            }

            return Ok(data.Data);

        }
    }
}
