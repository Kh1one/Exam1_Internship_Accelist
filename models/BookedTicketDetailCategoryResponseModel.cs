namespace exam1.models
{
    public class BookedTicketDetailCategoryResponseModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int SummaryPrice { get; set; }
        public List<BookedTicketDetailResponseModel>? Tickets { get; set; }
    }
}
