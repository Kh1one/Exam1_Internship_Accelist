using exam1.models;
using exam1.services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exam1.Controllers
{
    [Route("api/v1/book-ticket")]
    [ApiController]
    public class InsertBookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _services;
        public InsertBookedTicketController(BookedTicketService services)
        {
            _services = services;
        }

        // GET: api/<BookedTicketController>
        [HttpPost]
        public async Task<IActionResult> InsertNewBookedTicket([FromBody] List<BookedTicketDetailRequestModel> request)
        {
            if (ModelState.IsValid == false)
            {
                throw new ArgumentException("Failed to inserts due to invalid model state");
            }

            var data = await _services.InsertNewBookedTicket(request);

            return Ok(data);
        }
    }
}
