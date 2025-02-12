using exam1.models;
using exam1.services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace exam1.Controllers
{
    [Route("api/v1/edit-booked-ticket/{BookedTicketId}")]
    [ApiController]
    public class UpdateBookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _services;
        public UpdateBookedTicketController(BookedTicketService services)
        {
            _services = services;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBookedTicket([FromBody] List<UpdateBookedTicketRequestModel> request, int BookedTicketId)
        {
            if (ModelState.IsValid == false)
            {
                throw new ArgumentException("Failed to inserts due to invalid model state");
            }

            var data = await _services.UpdateBookedTicketDetails(request, BookedTicketId);

            return Ok(data);
        }
    }
}
