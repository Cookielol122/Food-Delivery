namespace FoodDelivery.BLL.Dto
{
    public class CartDto : Interfaces.IModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime PutDate { get; set; }
        public string ProductPhoto { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public bool IsOrdered { get; set; }
        public int ItemsPrice { get; set; }
    }
}
