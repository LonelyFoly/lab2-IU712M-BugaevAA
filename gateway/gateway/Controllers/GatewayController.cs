using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace gateway.Controllers
{
    
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
            var response = await client.GetAsync(
                $"http://loyalty:8070/api/v1/loyalty");
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return Ok(content);
            }

            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("/api/v1/reservations")]
        public async Task<IActionResult> InfoHotel([FromBody] DateForm df)
        {
            TimeSpan difference = df.endDate- df.startDate;

            // Получаем количество дней
            double totalDays = difference.TotalDays;

                
            var client = _clientFactory.CreateClient();
            
            var response = await client.GetAsync($"http://reservation:8060/api/v1/hotels/{df.hotelUid}");
            var content = await response.Content.ReadAsStringAsync();



            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 1");
                return NotFound();
            }
            hotel _hotel = JsonSerializer.Deserialize<hotel>(content);
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response2 = await client.GetAsync($"http://loyalty:8070/api/v1/loyalty");
            var content2 = await response2.Content.ReadAsStringAsync();


            if (!response2.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 2");
                return NotFound();
            }
                    
            loyalty _loyalty = JsonSerializer.Deserialize<loyalty>(content2);
            double q = _loyalty.discount;
            Console.WriteLine($"q = {q}, _hotel.price= {_hotel.price}," +
                $"totalDays = {totalDays}");
            int price_sum = Convert.ToInt32 (
                Math.Round((100 - q) * _hotel.price * totalDays)
                );
            Guid payment_uid = Guid.NewGuid();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var json = JsonSerializer.Serialize(new PaymentToDo(payment_uid, price_sum));

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response3 = await client.GetAsync($"http://payment:8050/api/v1/payment/{payment_uid}/{price_sum}");
            var content3 = await response3.Content.ReadAsStringAsync();
            if (!response3.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 3");
                return NotFound();
            }
            var response4 = await client.GetAsync($"http://loyalty:8070/api/v1/loyaltyInc");
            var content4 = await response4.Content.ReadAsStringAsync();
            if (!response4.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 4");
                return NotFound();
            }
            
            var response5 = await client.PostAsync($"http://reservation:8060/api/v1/loyaltyInc");
            var content5 = await response5.Content.ReadAsStringAsync();
            return Ok(content5);
            
        }
        [HttpDelete("api/v1/reservations/{reservationUid}")]
        public async Task<IActionResult> CancelReservation(Guid reservationUid)
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.DeleteAsync($"http://reservation:8060/api/v1/reservation/{reservationUid}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Reservation====response 1");
                return NotFound();
            }
            reservation _res = JsonSerializer.Deserialize<reservation>(content);
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response2 = await client.PatchAsync(
                $"http://payment:8050/api/v1/payment/{_res.payment_uid}", null);
            var content2 = await response2.Content.ReadAsStringAsync();


            if (!response2.IsSuccessStatusCode)
            {
                Console.WriteLine("Reservation====response 2");
                return NotFound();
            }

            
            var response4 = await client.GetAsync($"http://loyalty:8070/api/v1/loyaltyDec");
            var content4 = await response4.Content.ReadAsStringAsync();
            return Ok(content4);
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

    }
}