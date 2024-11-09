namespace payment
{
    public class payment
    {
        public int id { get; set; }
        public Guid payment_uid { get; set; }
        public string status { get; set; }
        public int price { get; set; }

        public payment()
        {

        }
    }
}
