using CineLog.Data.Models;
using CineLog.BLL.DTO;
using Microsoft.EntityFrameworkCore;

namespace CineLog.BLL.Services
{
    public class EfCineLogService : ICineLogService
    {
        private readonly CineLogContext _context;

        public EfCineLogService(CineLogContext context)
        {
            _context = context;
        }

        // 1. Top Rated
        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync()
        {
            return await _context.Movies
                .OrderByDescending(m => m.VoteAverage)
                .ThenByDescending(m => m.VoteCount)
                .Take(100)
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. Movies by genre
        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genreName)
        {
            return await _context.Movies
                .Where(m => m.Genres.Any(g => g.GenreName == genreName))
                .OrderByDescending(m => m.VoteAverage)
                .ThenByDescending(m => m.VoteCount)
                .Take(100)
                .AsNoTracking()
                .ToListAsync();
        }


        // 3. Movie details
        public async Task<MovieDetailsDTO?> GetMovieDetailsAsync(int movieId)
        {
            var movie = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
                return null;

            return new MovieDetailsDTO
            {
                MovieID = movie.MovieId,
                Title = movie.Title ?? "",
                ReleaseYear = movie.ReleaseYear,
                VoteAverage = movie.VoteAverage ?? 0,
                VoteCount = movie.VoteCount ?? 0,

                Genres = movie.Genres.Select(g => g.GenreName ?? "").ToList(),

                RecentReviews = movie.Reviews
                    .OrderByDescending(r => r.ReviewDate)
                    .Take(20)
                    .Select(r => new ReviewDTO
                    {
                        UserName = r.User?.UserName ?? "",
                        Rating = r.Rating ?? 0,
                        ReviewText = r.ReviewText ?? "",
                        ReviewDate = r.ReviewDate
                    }).ToList()
            };
        }

        // 4. User activity
        public async Task<IEnumerable<UserActivityDTO>> GetUserActivityAsync(int userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .Select(r => new UserActivityDTO
                {
                    ActivityType = "Review",
                    MovieTitle = r.Movie.Title ?? "",
                    Rating = r.Rating,
                    ActivityDate = r.ReviewDate
                }).ToListAsync();

            var watchlist = await _context.Watchlists
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId)
                .Select(w => new UserActivityDTO
                {
                    ActivityType = "Watchlist",
                    MovieTitle = w.Movie.Title ?? "",
                    Rating = null,
                    ActivityDate = w.AddedDate ?? DateTime.MinValue
                }).ToListAsync();

            var listItems = await _context.ListItems
                .Include(li => li.List)
                .Include(li => li.Movie)
                .Where(li => li.List.UserId == userId)
                .Select(li => new UserActivityDTO
                {
                    ActivityType = "List Item",
                    MovieTitle = li.Movie.Title ?? "",
                    Rating = null,
                    ActivityDate = li.AddedDate ?? DateTime.MinValue
                }).ToListAsync();

            return reviews
                .Concat(watchlist)
                .Concat(listItems)
                .OrderByDescending(a => a.ActivityDate)
                .ToList();
        }

        // 5. Trending
        public async Task<IEnumerable<Movie>> GetTrendingMoviesAsync(int days)
        {
            DateTime since = DateTime.Now.AddDays(-days);

            return await _context.Reviews
                .Where(r => r.ReviewDate >= since)
                .GroupBy(r => r.MovieId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.First().Movie!)
                .AsNoTracking()
                .ToListAsync();
        }

        // 6. Recommendations
        public async Task<IEnumerable<Movie>> RecommendMoviesAsync(int userId)
        {
            var topGenres = await _context.Reviews
                .Where(r => r.UserId == userId)
                .SelectMany(r => r.Movie.Genres)
                .GroupBy(g => g.GenreId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.Movies
                .Where(m => m.Genres.Any(g => topGenres.Contains(g.GenreId)))
                .OrderByDescending(m => m.VoteAverage)
                .AsNoTracking()
                .ToListAsync();
        }

        // 7. Friend connections
        public async Task<IEnumerable<FriendConnectionDTO>> GetFriendConnectionsAsync(int userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => new FriendConnectionDTO
                {
                    UserName = f.Followed.UserName ?? "",
                    Level = 1
                })
                .ToListAsync();
        }
    }
}
