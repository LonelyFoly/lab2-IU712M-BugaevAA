
using reservation.DB;
using Microsoft.AspNetCore.Mvc;

namespace reservation.Controllers
{
    [ApiController]

    public class ReservationController : ControllerBase
    {
        dbHandler handler;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(ILogger<ReservationController> logger)
        {
            _logger = logger;
            handler = new dbHandler(null);
        }

        [HttpGet("/api/v1/hotels&page={page}&size={size}")]
        public IActionResult GetHotels(int page= 1, int size = 10)
        {
            var hotels = handler.getHotels(page, size);
            return Ok(hotels);
        }
        [HttpGet("/api/v1/reservations")]
        public IActionResult GetReservations()
        {
            if (!Request.Headers.TryGetValue("X-User-Name", out var username))
            {
                return BadRequest("X-User-Name header is missing.");
            }

            var reservations = handler.getReservationsByUsername(username);
            return Ok(reservations);
        }
        [HttpGet("/api/v1/reservation/{reservationUid}")]
        public IActionResult GetReservations(Guid reservationUid)
        {
            if (!Request.Headers.TryGetValue("X-User-Name", out var username))
            {
                return BadRequest("X-User-Name header is missing.");
            }

            var reservation = handler.getReservationsByUsernameAndUid(reservationUid,username);
            if (!(reservation.username == ""))
                return Ok(reservation);
            return NotFound();
        }
        [HttpGet("/api/v1/me")]
        public IActionResult GetMe()
        {
            if (!Request.Headers.TryGetValue("X-User-Name", out var username))
            {
                return BadRequest("X-User-Name header is missing.");
            }
            
            var reservations = handler.getReservationsByUsername(username);
            string status = "GOLD";
            var result = new
            {
                reservations, // Добавляем весь объект класса
                status // Добавляем отдельную переменную
            };
            return Ok(result);
        }
        






    }
}
