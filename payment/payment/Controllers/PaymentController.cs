
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
        [HttpGet("/manage/health")]
        public IActionResult CheckHealth()
        {
            return Ok();
        }
        [HttpGet("/api/v1/payment/{payment_uid}/{price}")]
        public IActionResult PostPayment(Guid payment_uid, int price)
        {
            handler.addPayment(payment_uid, price);
            return Ok();
        }
        [HttpPatch("/api/v1/payment/{payment_uid}")]
        public IActionResult CancelPayment(Guid payment_uid)
        {
            payment _ = handler.cancelPayment(payment_uid);
            return Ok(_);
        }





    }
}
