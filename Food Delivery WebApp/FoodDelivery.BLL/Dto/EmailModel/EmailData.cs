namespace FoodDelivery.BLL.Dto.EmailModel
{
    public class EmailData
    {
        public string Body { get; set; }
        public string DeliveryAddress { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; } = string.Empty;
        public System.DateTime SendingDate { get; set; } = System.DateTime.MinValue;
    }
}
