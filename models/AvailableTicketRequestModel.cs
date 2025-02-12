using System.ComponentModel.DataAnnotations;

namespace exam1.models
{
    public class AvailableTicketRequestModel
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime MinEventDate { get; set; } = DateTime.MinValue;
        public DateTime MaxEventDate { get; set; } = DateTime.MinValue;
        public string OrderBy { get; set; } = string.Empty;
        public string OrderState { get; set; } = string.Empty;
    }
}
