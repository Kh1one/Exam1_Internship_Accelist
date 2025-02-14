namespace exam1.models
{
    public class UpdateBookedTicketRequestModel
    {
        public string TicketCode { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
