namespace FoodDelivery.BLL.Dto
{
    public class CategoryDto : Interfaces.IModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Photo { get; set; }
        public string TagName { get; set; }
    }
}
