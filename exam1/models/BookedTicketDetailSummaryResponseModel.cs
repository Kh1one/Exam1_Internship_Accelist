namespace exam1.models
{
    public class BookedTicketDetailSummaryResponseModel
    {
        public int PriceSummary { get; set;}

        public List<BookedTicketDetailCategoryResponseModel>? TicketsPerCategory { get; set;}
    }
}
