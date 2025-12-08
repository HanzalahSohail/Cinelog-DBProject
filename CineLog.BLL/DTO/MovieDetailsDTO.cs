namespace CineLog.BLL.DTO
{
    public class MovieDetailsDTO
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public int? ReleaseYear { get; set; }
        public decimal? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<ReviewDTO> RecentReviews { get; set; }
    }

    public class ReviewDTO
    {
        public string UserName { get; set; }
        public decimal Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
