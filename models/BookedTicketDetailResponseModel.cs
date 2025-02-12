namespace exam1.models
{
    public class BookedTicketDetailResponseModel
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set;} = string.Empty;
        public int Price{ get; set;}
    }
}
