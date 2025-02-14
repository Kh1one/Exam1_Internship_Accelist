namespace exam1.models
{
    public class GetBookedTicketModel
    {
        public string TicketCode { get; set; } = string.Empty;

        public string TicketName { get; set;} = string.Empty;

        //public DateTime EventDate { get; set; }

        public string EventTime { get; set; } = string.Empty ;
    }
}
