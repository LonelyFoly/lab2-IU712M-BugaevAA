
using loyalty.DB;
using Microsoft.AspNetCore.Mvc;

namespace loyalty.Controllers
{
    [ApiController]

    public class LoyaltyController : ControllerBase
    {
        dbHandler handler;
        private readonly ILogger<LoyaltyController> _logger;

        public LoyaltyController(ILogger<LoyaltyController> logger)
        {
            _logger = logger;
            handler = new dbHandler(null);
        }

        [HttpGet("/api/v1/loyalty")]
        public IActionResult GetLoyalty()
        {
            if (!Request.Headers.TryGetValue("X-User-Name", out var username))
            {
                return BadRequest("X-User-Name header is missing.");
            }
            var loyalty = handler.getLoyalty(username);
            return Ok(loyalty);
        }

        




    }
}
