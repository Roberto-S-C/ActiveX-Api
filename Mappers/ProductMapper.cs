using ActiveX_Api.Dto.Product;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class ProductMapper
    {
        public static ProductsListDto FromProductToProductListDto (this Product product)
        {
            return new ProductsListDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                File3DModel = product.File3DModel,
                Category = product.Category
            }; 
        }

        public static Product FromCreateProductDtoToProduct (this CreateProductDto product)
        {
            return new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                CategoryId = product.CategoryId
            };
        }  

        public static GetProductDto FromProductToGetProductDto (this Product product)
        {
            return new GetProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                File3DModel = product.File3DModel,
                StripePriceId = product.StripePriceId,
                Category = product.Category.FromCategoryToCategoryListDto(),
                Reviews = product.Reviews.Select(r => r.FromReviewToReviewListDto()).ToList()
            };
        }
    }
}
