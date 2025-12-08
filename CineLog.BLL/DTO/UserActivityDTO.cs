namespace CineLog.BLL.DTO
{
    public class UserActivityDTO
    {
        public string ActivityType { get; set; }   // Review / Watchlist / List Item
        public string MovieTitle { get; set; }
        public decimal? Rating { get; set; }       // Only for reviews
        public DateTime ActivityDate { get; set; }
    }
}
