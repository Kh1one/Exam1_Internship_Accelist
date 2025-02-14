namespace exam1.models
{
    public class BookedTicketDetailModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Price { get; set; }
        public string TicketCode { get; set; } = string.Empty ;
        public string TicketName { get; set;} = string.Empty ;
        public int Quantity { get; set; }

        public DateTime EventDate { get; set; }
    }
}
