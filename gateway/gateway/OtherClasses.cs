namespace gateway
{
    public class loyalty
    {
        public int id { get; set; }
        public string username { get; set; }
        public int reservation_count { get; set; }
        public string status { get; set; }
        public int discount { get; set; }

        public loyalty()
        {

        }
    }
    public class hotel
    {
        public int id { get; set; }
        public Guid hotel_uid { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public int stars { get; set; }
        public int price { get; set; }
        public hotel()
        {

        }

    }
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
