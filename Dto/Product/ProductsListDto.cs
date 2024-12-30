namespace ActiveX_Api.Dto.Product
{
    using ActiveX_Api.Models;
    public class ProductsListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? File3DModel { get; set; }
        public Category Category { get; set; }
    }
}
