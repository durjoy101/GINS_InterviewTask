namespace GINS_Task.Models
{
    public class MeetingMinutesViewModel
    {
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MeetingPlace { get; set; }
        public string AttendsClient { get; set; }
        public string AttendsHost { get; set; }
        public string MeetingAgenda { get; set; }
        public string MeetingDiscussion { get; set; }
        public string MeetingDecision { get; set; }
        public List<MeetingMunitesProduct> MeetingMunitesProducts { get; set; } = new List<MeetingMunitesProduct>();
    }

    public class MeetingMunitesProduct
    {
        public string ProductService { get; set; }
        public int Quantity { get; set; }
        public int Unit { get; set; }
    }
}
