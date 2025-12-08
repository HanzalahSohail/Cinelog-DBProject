using Microsoft.AspNetCore.Mvc;
using CineLog.Data.Models;
using CineLog.BLL.Services;

namespace CineLog.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly CineLogContext _context;

        public UsersController(CineLogContext context)
        {
            _context = context;
        }

        private ICineLogService GetService()
        {
            return CineLogServiceFactory.Create(_context);
        }

        // GET api/users/activity/12
        [HttpGet("activity/{userId}")]
        public async Task<IActionResult> GetUserActivity(int userId)
        {
            var service = GetService();
            return Ok(await service.GetUserActivityAsync(userId));
        }

        // GET api/users/connections/12
        [HttpGet("connections/{userId}")]
        public async Task<IActionResult> GetConnections(int userId)
        {
            var service = GetService();
            return Ok(await service.GetFriendConnectionsAsync(userId));
        }
    }
}
