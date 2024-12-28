namespace ActiveX_Api.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int stars {  get; set; }
        public DateTime? Created { get; set; } = default(DateTime?);
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
