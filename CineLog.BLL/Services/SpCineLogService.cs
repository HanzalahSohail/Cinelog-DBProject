using CineLog.BLL.DTO;
using CineLog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineLog.BLL.Services
{
    public class SpCineLogService : ICineLogService
    {
        private readonly CineLogContext _context;

        public SpCineLogService(CineLogContext context)
        {
            _context = context;
        }

        // 1. Top Rated - using view is tricky because it lacks MovieID.
        // For now we just mimic EF version with direct SQL.
        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync()
        {
            return await _context.Movies
                .FromSqlRaw("SELECT TOP 100 * FROM Movies ORDER BY VoteAverage DESC, VoteCount DESC")
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. Movies by Genre - uses Proced_GetMoviesByGenre
        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genreName)
        {
            return await _context.Movies
                .FromSqlInterpolated($"EXEC Proced_GetMoviesByGenre {genreName}")
                .AsNoTracking()
                .ToListAsync();
        }

        // 3. Movie Details (SP returns 3 result sets).
        // Full mapping from multi-result-set SP via EF is messy.
        // Keep EF mode for this; SP mode throws for now.
        public Task<MovieDetailsDTO?> GetMovieDetailsAsync(int movieId)
        {
            throw new NotImplementedException("SP version of GetMovieDetailsAsync not implemented. Use EF mode.");
        }

        // 4. User Activity - SP version deferred
        public Task<IEnumerable<UserActivityDTO>> GetUserActivityAsync(int userId)
        {
            throw new NotImplementedException("SP version of GetUserActivityAsync not implemented. Use EF mode.");
        }

        // 5. Trending Movies
        public async Task<IEnumerable<Movie>> GetTrendingMoviesAsync(int days)
        {
            return await _context.Movies
                .FromSqlInterpolated($"EXEC proced_GetTrendingMovies {days}")
                .AsNoTracking()
                .ToListAsync();
        }

        // 6. Recommend Movies
        public async Task<IEnumerable<Movie>> RecommendMoviesAsync(int userId)
        {
            return await _context.Movies
                .FromSqlInterpolated($"EXEC proced_RecommendMovies {userId}")
                .AsNoTracking()
                .ToListAsync();
        }

        // 7. Friend Connections
        public async Task<IEnumerable<FriendConnectionDTO>> GetFriendConnectionsAsync(int userId)
        {
            // No mapped entity for SP result; leave SP mode unimplemented.
            throw new NotImplementedException("SP version of GetFriendConnectionsAsync not implemented. Use EF mode.");
        }

        // ---------------- REVIEWS ----------------
        public async Task<bool> AddReviewAsync(ReviewCreateDTO dto)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC proced_AddReview {dto.UserId}, {dto.MovieId}, {dto.Rating}, {dto.ReviewText}"
            );
            return true;
        }

        // ---------------- WATCHLIST ----------------
        public async Task<bool> AddToWatchlistAsync(int userId, int movieId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO Watchlist (UserID, MovieID) VALUES ({userId}, {movieId})"
            );
            return true;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int userId, int movieId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM Watchlist WHERE UserID = {userId} AND MovieID = {movieId}"
            );
            return true;
        }

        public async Task<IEnumerable<Movie>> GetWatchlistAsync(int userId)
        {
            return await _context.Movies
                .FromSqlInterpolated(
                    $"SELECT m.* FROM Movies m JOIN Watchlist w ON m.MovieID = w.MovieID WHERE w.UserID = {userId}"
                )
                .AsNoTracking()
                .ToListAsync();
        }

        // ---------------- LISTS ----------------
        public async Task<IEnumerable<List>> GetUserListsAsync(int userId)
        {
            return await _context.Lists
                .FromSqlInterpolated($"SELECT * FROM Lists WHERE UserID = {userId}")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateListAsync(ListCreateDTO dto)
        {
            // Simple insert; retrieve identity with SCOPE_IDENTITY()
            var sql = @"
                INSERT INTO Lists (UserID, Title, Description, CreatedDate)
                VALUES (@p0, @p1, @p2, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            var result = await _context.Database
                .SqlQueryRaw<int>(sql, dto.UserId, dto.Title, dto.Description)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<bool> AddListItemAsync(int listId, int movieId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO ListItem (ListID, MovieID) VALUES ({listId}, {movieId})"
            );
            return true;
        }

        public async Task<bool> RemoveListItemAsync(int listId, int movieId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM ListItem WHERE ListID = {listId} AND MovieID = {movieId}"
            );
            return true;
        }

        public async Task<IEnumerable<Movie>> GetListItemsAsync(int listId)
        {
            return await _context.Movies
                .FromSqlInterpolated(
                    $"SELECT m.* FROM Movies m JOIN ListItem li ON m.MovieID = li.MovieID WHERE li.ListID = {listId}"
                )
                .AsNoTracking()
                .ToListAsync();
        }

        // ---------------- SEARCH ----------------
        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string query)
        {
            query ??= string.Empty;
            return await _context.Movies
                .FromSqlInterpolated($"SELECT * FROM Movies WHERE Title LIKE '%' + {query} + '%'")
                .AsNoTracking()
                .ToListAsync();
        }

        // ---------------- ANALYTICS ----------------
        public Task<IEnumerable<SuperUserDTO>> GetSuperUsersAsync()
        {
            throw new NotImplementedException("SP version of GetSuperUsersAsync not implemented. Use EF mode.");
        }

        public Task<IEnumerable<PopularGenreDTO>> GetPopularGenresAsync()
        {
            throw new NotImplementedException("SP version of GetPopularGenresAsync not implemented. Use EF mode.");
        }

        public Task<IEnumerable<ActivityFeedDTO>> GetPublicActivityFeedAsync()
        {
            throw new NotImplementedException("SP version of GetPublicActivityFeedAsync not implemented. Use EF mode.");
        }
    }
}
