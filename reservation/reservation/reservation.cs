using System;

namespace reservation
{
    public class reservation
    {
        public int id { get; set; }
        public Guid reservation_uid { get; set; }
        public string username { get; set; }
        public Guid payment_uid { get; set; }
        public int hotel_id { get; set; }
        public string status { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_data { get; set; }

        public reservation()
        {

        }
    }
}
