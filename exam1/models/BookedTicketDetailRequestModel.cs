using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace exam1.models
{
    public class BookedTicketDetailRequestModel
    {
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public int Quantity{ get; set; }
    }
}
