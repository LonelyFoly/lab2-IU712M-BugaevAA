using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace gateway.Controllers
{
    public class DateForm
    {
        public DateTime start_date { get; set; }
        public DateTime end_data { get; set; }
        public DateForm() { }
    }
    [ApiController]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        string username;
        public GatewayController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            username = "Test Max";
        }


        [HttpGet("/api/v1/hotels&page={page}&size={size}")]
        public async Task<IActionResult> ProxyReservationService(int page, int size)
        {

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"http://reservation:8060/api/v1/hotels&page={page}&size={size}");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, content);
        }
        [HttpGet("/api/v1/reservations")]
        public async Task<IActionResult> ReservateMe()
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/me");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, content);
        }
        [HttpGet("/api/v1/reservations/{reservationUid}")]
        public async Task<IActionResult> CheckReservateMe(Guid reservationUid)
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/reservation/{reservationUid}");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return NotFound();
        }
        [HttpGet("api/v1/loyalty")]
        public async Task<IActionResult> GetLoyalty()
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://loyalty:8070/api/v1/loyalty");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("/api/v1/hotels/{hotelUid}")]
        public async Task<IActionResult> InfoHotel([FromBody] DateForm date, Guid hotelUid)
        {
            TimeSpan difference = date.end_data - date.start_date;

            // Получаем количество дней
            double totalDays = difference.TotalDays;


            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://loyalty:8070/api/v1/loyalty");
            var content = await response.Content.ReadAsStringAsync();

            

            if (response.IsSuccessStatusCode)
            {
                List<loyalty> _loyalty = JsonSerializer.Deserialize<List<loyalty>>(content);
                switch (_loyalty[0].status)
                {
                    case "GOLD":
                        break;
                    case "BRONZE":
                        break;
                    case "SILVER":
                        break;
                    default:
                        break;
                }
                return Ok(content);
            }

            return NotFound();
        }
        [HttpGet("api/v1/me")]
        public async Task<IActionResult> ReservationMe()
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/me");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, content);
        }
        /*[HttpGet("payment/{*path}")]
        public async Task<IActionResult> ProxyPaymentService(string path)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:8050/{path}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        [HttpGet("loyalty/{*path}")]
        public async Task<IActionResult> ProxyLoyaltyService(string path)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"http://loyalty-service:8070/{path}");
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }*/
    }
}