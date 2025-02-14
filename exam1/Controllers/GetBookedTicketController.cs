using exam1.services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exam1.Controllers
{
    [Route("api/v1/get-booked-ticket/{BookedTicketId}")]
    [ApiController]
    public class GetBookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _services;
        public GetBookedTicketController(BookedTicketService services)
        {
            _services = services;
        }

        // GET: api/<GetBookedTicketController>
        [HttpGet]
        public async Task<IActionResult> Get(int BookedTicketId)
        {

            var data = await _services.GetBookedTicketDetails(BookedTicketId);

            if (data.Status != 0)
            {
                data.Instance = HttpContext.Request.Path;
                return Ok(data);
            }

            return Ok(data.Data);
        }
    }
}
