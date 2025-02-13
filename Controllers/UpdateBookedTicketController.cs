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
                return Ok(new ServiceResponse<BookedTicketDetailRequestModel>
                {
                    Data = null,
                    Title = "Invalid model state",
                    Status = 400,
                    Detail = "Failed to inserts due to invalid model state",
                    Instance = HttpContext.Request.Path
                });
                //throw new ArgumentException("Failed to inserts due to invalid model state");
            }

            var data = await _services.UpdateBookedTicketDetails(request, BookedTicketId);

            if (data.Status != 0)
            {
                data.Instance = HttpContext.Request.Path;
                return Ok(data);
            }

            return Ok(data.Data);
        }
    }
}
