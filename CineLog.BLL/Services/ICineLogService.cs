using CineLog.Data.Models;
using CineLog.BLL.DTO;

namespace CineLog.BLL.Services
{
    public interface ICineLogService
    {
        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync();
        Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genreName);
        Task<MovieDetailsDTO?> GetMovieDetailsAsync(int movieId);
        Task<IEnumerable<UserActivityDTO>> GetUserActivityAsync(int userId);
        Task<IEnumerable<Movie>> GetTrendingMoviesAsync(int days);
        Task<IEnumerable<Movie>> RecommendMoviesAsync(int userId);
        Task<IEnumerable<FriendConnectionDTO>> GetFriendConnectionsAsync(int userId);
    }
}
