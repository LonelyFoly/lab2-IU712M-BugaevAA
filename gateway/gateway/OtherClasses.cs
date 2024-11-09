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
    public class DateForm
    {
        public Guid hotelUid { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateForm() { }
    }
    public class PaymentToDo
    {
        public Guid payment_uid { get; set; }
        public int price { get; set; }
        public PaymentToDo() { }
        public PaymentToDo(Guid _payment_uid, int _price) {
            payment_uid = _payment_uid;
            price = _price;
        }
    }
}
