using exam1.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam1.Controllers
{
    [Route("api/v1/revoke-ticket/{BookedTicketId}/{TicketCode}/{Quantity}")]
    [ApiController]
    public class DeleteBookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _services;
        public DeleteBookedTicketController(BookedTicketService services)
        {
            _services = services;
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeleteBookedTicket(int BookedTicketId, string TicketCode, int Quantity)
        {

            var data = await _services.DeleteBookedTicket(BookedTicketId, TicketCode, Quantity);

            return Ok(data);
        }
    }
}
