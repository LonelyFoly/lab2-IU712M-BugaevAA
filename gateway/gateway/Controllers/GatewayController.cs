using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        [HttpGet("/manage/health")]
        public IActionResult CheckHealth()
        {
            return Ok();
        }
        [HttpGet("/api/v1/hotels")]
        public async Task<IActionResult> ProxyReservationService([FromQuery] int page, [FromQuery] int size)
        {

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"http://reservation:8060/api/v1/hotels?page={page}&size={size}");
            var content = await response.Content.ReadFromJsonAsync<hotel[]>();



            if (response.IsSuccessStatusCode)
            {
                /*var result = new
                {
                    items = content,
                    content[0].id,
                    content[0].name,
                    content[0].city,
                    content[0].country,
                    content[0].address,
                    content[0].hotelUid,
                    content[0].price,
                    content[0].stars

                };*/
                var result = new
                {
                    items = content.Select(item => new
                    {
                        item.id,
                        item.name,
                        item.city,
                        item.country,
                        item.address,
                        item.hotelUid,
                        item.price,
                        item.stars
                    }).ToList(),
                    page=page,
                    pageSize =size,
                    totalElements =content.Count() 
                };
                return Ok(result);
            }

            return StatusCode((int)response.StatusCode, content);
        }
        [HttpGet("/api/v1/reservations")]
        public async Task<IActionResult> ReservateMe()
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/reservations");
            var content = await response.Content.ReadFromJsonAsync<reservation[]>();

            //

            var _hotels = new Dictionary<string, hotel>();
            var _payments = new Dictionary<string, payment>();

            foreach (var item in content)
            {
                if (!_hotels.ContainsKey(item.hotelUid.ToString()))
                {
                    var response1 = await client.GetAsync($"http://reservation:8060/api/v1/hotels/{item.hotelUid}");
                    if (response1.IsSuccessStatusCode)
                    {
                        var content1 = await response1.Content.ReadAsStringAsync();
                        _hotels[item.hotelUid.ToString()] = JsonSerializer.Deserialize<hotel>(content1);
                        Console.WriteLine($"    hotelUid: {item.hotelUid}");
                    }
                    else
                    {
                        Console.WriteLine($"====response 1 failed for hotelUid: {item.hotelUid}");
                        return NotFound();
                    }
                }
                if (!_payments.ContainsKey(item.paymentUid.ToString()))
                {
                    var response3 = await client.GetAsync($"http://payment:8050/api/v1/payment/{item.paymentUid}");
                    Console.WriteLine($"    paymentUUid: {item.paymentUid}");
                    if (!response3.IsSuccessStatusCode)
                    {
                        Console.WriteLine("====response 2");
                        return NotFound();
                    }
                    var content3 = await response3.Content.ReadAsStringAsync();
                    _payments[item.paymentUid.ToString()] = JsonSerializer.Deserialize<payment>(content3);

                }
            }
            


            if (response.IsSuccessStatusCode)
            {
                var result = content.Select(item => new
                {
                    reservationUid = item.reservationUid,
                    hotel = new {
                        hotelUid = _hotels[item.hotelUid.ToString()].hotelUid,
                        name = _hotels[item.hotelUid.ToString()].name,
                        fullAddress = _hotels[item.hotelUid.ToString()].country +", "+_hotels[item.hotelUid.ToString()].city+", " +_hotels[item.hotelUid.ToString()].address,
                        stars = _hotels[item.hotelUid.ToString()].stars,
                    },
                   
                    startDate = item.startDate.ToString("yyyy-MM-dd"),
                    endDate = item.endDate.ToString("yyyy-MM-dd"),
                    status = item.status,
                    payment = new {
                        status = _payments[item.paymentUid.ToString()].status,
                        price = _payments[item.paymentUid.ToString()].price
                }
                }).ToList();
                return Ok(result);
            }

            return StatusCode((int)response.StatusCode, content);
        }
        [HttpGet("/api/v1/reservations/{reservationUid}")]
        public async Task<IActionResult> CheckReservateMe(Guid reservationUid)
        {

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/reservation/{reservationUid}");
            var content = await response.Content.ReadFromJsonAsync<reservation>();

            var response2 = await client.GetAsync($"http://reservation:8060/api/v1/hotels/{content.hotelUid}");
            var content2 = await response2.Content.ReadAsStringAsync();
            if (!response2.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 2");
                Console.WriteLine($"{content.hotelUid}");
                return NotFound();
            }
            hotel _hotel = JsonSerializer.Deserialize<hotel>(content2);



            var response3 = await client.GetAsync($"http://payment:8050/api/v1/payment/{content.paymentUid}");

            if (!response3.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 3");
                return NotFound();
            }
            var content3 = await response3.Content.ReadAsStringAsync();
            payment payment = JsonSerializer.Deserialize<payment>(content3);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    hotel = new {
                        hotelUid = content.hotelUid,
                        name = _hotel.name,
                        fullAddress = _hotel.country +", "+
                        _hotel.city+", " +
                        _hotel.address,
                        stars = _hotel.stars
                    },
                    reservationUid = content.reservationUid,
                    startDate = content.startDate.ToString("yyyy-MM-dd"),
                    endDate = content.endDate.ToString("yyyy-MM-dd"),
                    status = content.status,
                    payment = new
                    {
                        status = payment.status,
                        price = payment.price
                    }

                });
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
            var content = await response.Content.ReadFromJsonAsync<loyalty>();


            if (response.IsSuccessStatusCode)
            {
                var result = new
                {
                    status = content.status,
                    discount = content.discount,
                    reservationCount = content.reservationCount,
                };
                return Ok(result);

            }

            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("/api/v1/reservations")]
        public async Task<IActionResult> PostReservation([FromBody] DateForm df)
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
                Math.Round((100 - q)*0.01 * _hotel.price * totalDays)
                );
            Console.WriteLine($"price_sum = {price_sum}");
            Guid paymentUid = Guid.NewGuid();
            var json = JsonSerializer.Serialize(new PaymentToDo(paymentUid, price_sum));

            //передаёт информацию о новой оплате
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response3 = await client.PostAsync($"http://payment:8050/api/v1/payment", httpContent);
            var content3 = await response3.Content.ReadAsStringAsync();
            if (!response3.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 3");
                return NotFound(content3);
            }
            var response4 = await client.PatchAsync(
                $"http://loyalty:8070/api/v1/loyaltyInc", null
                );
            var content4 = await response4.Content.ReadAsStringAsync();
            if (!response4.IsSuccessStatusCode)
            {
                Console.WriteLine("====response 4");
                return NotFound();
            }

            //add reservation
            Guid reservationUid = Guid.NewGuid();
            json = JsonSerializer.Serialize(new reservation(
                reservationUid,
                username,
                paymentUid,
                _hotel.hotelUid,
                "PAID",
                df.startDate,
                df.endDate
                ));

            var sendContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response5 = await client.PostAsync($"http://reservation:8060/api/v1/reservation", sendContent);
            var content5 = await response5.Content.ReadAsStringAsync();

            return Ok( new
            {
                reservationUid= reservationUid,
                hotelUid = _hotel.hotelUid,
                startDate = df.startDate.ToString("yyyy-MM-dd"),
                endDate = df.endDate.ToString("yyyy-MM-dd"),
                discount = _loyalty.discount,
                status = "PAID",
                payment = new
                {
                    status = "PAID",
                    price = price_sum,
                }
            }
                
                );
            
        }
        [HttpDelete("api/v1/reservations/{reservationUid}")]
        public async Task<IActionResult> CancelReservation(Guid reservationUid)
        {
            //обращение к reservation -> статус по uuid: CANCELED
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.PatchAsync(
                $"http://reservation:8060/api/v1/reservation/{reservationUid}", null
                );
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
               // Console.WriteLine("!Reservation====response 1");

                return NotFound(content);
            }

            reservation _res = JsonSerializer.Deserialize<reservation>(content);

            var response2 = await client.PatchAsync(
                $"http://payment:8050/api/v1/payment/{_res.paymentUid}", null
                );
            var content2 = await response2.Content.ReadAsStringAsync();
            if (!response2.IsSuccessStatusCode)
            {
                //Console.WriteLine("Reservation====response 2");
                return NotFound(content2);
            }


            var response3 = await client.PatchAsync(
                "http://loyalty:8070/api/v1/loyaltyDecrease", null
                );
            var content3 = await response3.Content.ReadAsStringAsync();
            if (!response3.IsSuccessStatusCode)
            {
                //Console.WriteLine("!!!!!Reservation====response 2");
                return NotFound(content3);
            }

            return NoContent();
        }

            [HttpGet("api/v1/me")]
        public async Task<IActionResult> ReservationMe()
        {

/*            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/me");
            var content = await response.Content.ReadFromJsonAsync<reservation[]>();*/


            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-User-Name", username);
            var response = await client.GetAsync($"http://reservation:8060/api/v1/reservations");
            var content = await response.Content.ReadFromJsonAsync<reservation[]>();

            //

            var _hotels = new Dictionary<string, hotel>();
            var _payments = new Dictionary<string, payment>();

            foreach (var item in content)
            {
                if (!_hotels.ContainsKey(item.hotelUid.ToString()))
                {
                    var response1 = await client.GetAsync($"http://reservation:8060/api/v1/hotels/{item.hotelUid}");
                    if (response1.IsSuccessStatusCode)
                    {
                        var content1 = await response1.Content.ReadAsStringAsync();
                        _hotels[item.hotelUid.ToString()] = JsonSerializer.Deserialize<hotel>(content1);
                    }
                    else
                    {
                        Console.WriteLine($"====response 1 failed for hotelUid: {item.hotelUid}");
                        return NotFound();
                    }
                }
                if (!_payments.ContainsKey(item.paymentUid.ToString()))
                {
                    var response3 = await client.GetAsync($"http://payment:8050/api/v1/payment/{item.paymentUid}");
                    Console.WriteLine($"    paymentUUid: {item.paymentUid}");
                    if (!response3.IsSuccessStatusCode)
                    {
                        Console.WriteLine("====response 2");
                        return NotFound();
                    }
                    var content3 = await response3.Content.ReadAsStringAsync();
                    _payments[item.paymentUid.ToString()] = JsonSerializer.Deserialize<payment>(content3);

                }

            }

            var response4 = await client.GetAsync(
                $"http://loyalty:8070/api/v1/loyalty");
            var content4 = await response4.Content.ReadFromJsonAsync<loyalty>();




                if (response.IsSuccessStatusCode)
            {
                var result = new
                {
                    reservations = content.Select(item => new
                    {
                        reservationUid = item.reservationUid,
                        hotel = new
                        {
                            hotelUid = _hotels[item.hotelUid.ToString()].hotelUid,
                            name = _hotels[item.hotelUid.ToString()].name,
                            fullAddress = _hotels[item.hotelUid.ToString()].country + ", " + _hotels[item.hotelUid.ToString()].city + ", " + _hotels[item.hotelUid.ToString()].address,
                            stars = _hotels[item.hotelUid.ToString()].stars,
                        },

                        startDate = item.startDate.ToString("yyyy-MM-dd"),
                        endDate = item.endDate.ToString("yyyy-MM-dd"),
                        status = item.status,
                        payment = new
                        {
                            status = _payments[item.paymentUid.ToString()].status,
                            price = _payments[item.paymentUid.ToString()].price
                        }
                    }).ToList(),
                    loyalty = new
                    {
                        status = content4.status,
                        discount = content4.discount
                    }
                };
                return Ok(result)
                    ;
            }

            return StatusCode((int)response.StatusCode, content);
        }

    }
}