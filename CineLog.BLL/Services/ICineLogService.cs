using CineLog.BLL.DTO;
using CineLog.Data.Models;

namespace CineLog.BLL.Services
{
    public interface ICineLogService
    {
        // Phase 3 core queries
        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync();
        Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genreName);
        Task<MovieDetailsDTO?> GetMovieDetailsAsync(int movieId);
        Task<IEnumerable<UserActivityDTO>> GetUserActivityAsync(int userId);
        Task<IEnumerable<Movie>> GetTrendingMoviesAsync(int days);
        Task<IEnumerable<Movie>> RecommendMoviesAsync(int userId);
        Task<IEnumerable<FriendConnectionDTO>> GetFriendConnectionsAsync(int userId);

        // Extra: Reviews
        Task<bool> AddReviewAsync(ReviewCreateDTO dto);

        // Extra: Watchlist
        Task<bool> AddToWatchlistAsync(int userId, int movieId);
        Task<bool> RemoveFromWatchlistAsync(int userId, int movieId);
        Task<IEnumerable<Movie>> GetWatchlistAsync(int userId);

        // Extra: Lists
        Task<IEnumerable<List>> GetUserListsAsync(int userId);
        Task<int> CreateListAsync(ListCreateDTO dto);
        Task<bool> AddListItemAsync(int listId, int movieId);
        Task<bool> RemoveListItemAsync(int listId, int movieId);
        Task<IEnumerable<Movie>> GetListItemsAsync(int listId);

        // Search
        Task<IEnumerable<Movie>> SearchMoviesAsync(string query);

        // Analytics
        Task<IEnumerable<SuperUserDTO>> GetSuperUsersAsync();
        Task<IEnumerable<PopularGenreDTO>> GetPopularGenresAsync();
        Task<IEnumerable<ActivityFeedDTO>> GetPublicActivityFeedAsync();
    }
}
