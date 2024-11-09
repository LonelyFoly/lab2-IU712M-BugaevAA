namespace loyalty
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
    public class loyaltyToDo
    {
        public string username { get; set; }
        public int reservation_count { get; set; }
        public string status { get; set; }
        public int discount { get; set; }
        public loyaltyToDo()
        {

        }
    }
}
