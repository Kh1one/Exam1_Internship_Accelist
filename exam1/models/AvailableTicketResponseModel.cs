namespace exam1.models
{
    public class AvailableTicketResponseModel
    {
        public string EventDate { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public int Quota { get; set; }
        public int Price { get; set; }
        public string CategoryName {  get; set; } = string.Empty;

    }
}
