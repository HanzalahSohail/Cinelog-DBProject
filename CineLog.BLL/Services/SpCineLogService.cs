using CineLog.Data.Models;
using CineLog.BLL.DTO;
using Microsoft.EntityFrameworkCore;

namespace CineLog.BLL.Services
{
    public class SpCineLogService : ICineLogService
    {
        private readonly CineLogContext _context;
        private readonly EfCineLogService _efService;

        public SpCineLogService(CineLogContext context)
        {
            _context = context;
            _efService = new EfCineLogService(context);
        }

        // For simplicity & safety, we reuse the EF logic for this.
        public Task<IEnumerable<Movie>> GetTopRatedMoviesAsync()
        {
            return _efService.GetTopRatedMoviesAsync();
        }

        // Uses stored procedure Proced_GetMoviesByGenre
        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genreName)
        {
            return await _context.Movies
                .FromSqlRaw("EXEC Proced_GetMoviesByGenre @p0", genreName)
                .ToListAsync();
        }

        // Movie details are complex (joins + reviews), reuse EF
        public Task<MovieDetailsDTO> GetMovieDetailsAsync(int movieId)
        {
            return _efService.GetMovieDetailsAsync(movieId);
        }

        // User activity is DTO-heavy, reuse EF
        public Task<IEnumerable<UserActivityDTO>> GetUserActivityAsync(int userId)
        {
            return _efService.GetUserActivityAsync(userId);
        }

        // Uses stored procedure proced_GetTrendingMovies
        public async Task<IEnumerable<Movie>> GetTrendingMoviesAsync(int days)
        {
            return await _context.Movies
                .FromSqlRaw("EXEC proced_GetTrendingMovies @p0", days)
                .ToListAsync();
        }

        // Uses stored procedure proced_RecommendMovies
        public async Task<IEnumerable<Movie>> RecommendMoviesAsync(int userId)
        {
            return await _context.Movies
                .FromSqlRaw("EXEC proced_RecommendMovies @p0", userId)
                .ToListAsync();
        }

        // Friend connections are DTO-heavy, reuse EF
        public Task<IEnumerable<FriendConnectionDTO>> GetFriendConnectionsAsync(int userId)
        {
            return _efService.GetFriendConnectionsAsync(userId);
        }
    }
}
