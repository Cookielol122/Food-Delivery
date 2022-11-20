namespace FoodDelivery.BLL.Dto
{
    public class OrderDto : Interfaces.IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OrderProducts { get; set; }
        public int TotalAmount { get; set; }
    }
}
