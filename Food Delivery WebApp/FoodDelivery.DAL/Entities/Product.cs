namespace FoodDelivery.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public string Photo { get; set; }
        public string Code { get; set; }
        public System.DateTime DateAdded { get; set; }
        public bool IsPromotion { get; set; }
        public bool isStock { get; set; }
    }
}
