namespace FoodDelivery.DAL.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public System.DateTime DateMessage { get; set; }
        public string TextMessage { get; set; }
        public string Title { get; set; }
        public string TypeMessage { get; set; }
        public bool IsReviewed { get; set; }
    }
}
