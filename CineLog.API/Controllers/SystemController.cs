using Microsoft.AspNetCore.Mvc;
using CineLog.BLL.Services;
using CineLog.Data.Models;
using CineLog.BLL;


namespace CineLog.API.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly CineLogContext _context;

        public SystemController(CineLogContext context)
        {
            _context = context;
        }

        // GET api/system/mode
        [HttpGet("mode")]
        public IActionResult GetMode()
        {
            return Ok(new { mode = CineLogServiceFactory.GetMode() });
        }

        // POST api/system/mode/EF
        // POST api/system/mode/SP
        [HttpPost("mode/{mode}")]
        public IActionResult SetMode(string mode)
        {
            CineLogServiceFactory.SetMode(mode);
            return Ok(new { message = $"Mode switched to {CineLogServiceFactory.GetMode()}" });
        }
    }
}
