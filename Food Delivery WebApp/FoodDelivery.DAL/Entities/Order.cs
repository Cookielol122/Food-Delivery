namespace FoodDelivery.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderProducts { get; set; }
        public int TotalAmount { get; set; }
    }
}
