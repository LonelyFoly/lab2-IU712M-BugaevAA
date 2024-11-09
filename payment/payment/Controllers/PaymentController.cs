
using payment.DB;
using Microsoft.AspNetCore.Mvc;

namespace payment.Controllers
{
    [ApiController]

    public class PaymentController : ControllerBase
    {
        dbHandler handler;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
            handler = new dbHandler(null);
        }

        /*[HttpGet("/api/v1/loyalty")]
        public IActionResult GetLoyalty(string username)
        {
            var loyalty = handler.getLoyalty(username);
            return Ok(loyalty);
        }*/

        




    }
}
