namespace exam1.models
{
    public class GetBookedTicketCategoryModel
    {
        public int QuantityPerCategory { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public List<GetBookedTicketModel>? Tickets { get; set; }
    }
}
