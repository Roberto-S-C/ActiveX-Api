namespace ActiveX_Api.Dto.Review
{
    public class ReviewListDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Stars {  get; set; }
        public DateOnly CreatedAt { get; set; }
        public int ProductId { get; set; }
    }
}
