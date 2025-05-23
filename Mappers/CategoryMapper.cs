﻿using ActiveX_Api.Dto.Category;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class CategoryMapper
    {
        public static CategoriesListDto FromCategoryToCategoryListDto (this Category category)
        {
            return new CategoriesListDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public static Category FromCreateCategoryDtoToCategory (this CreateCategoryDto category)
        {
            return new Category
            {
                Name = category.Name,
            };
        }

        public static Category FromUpdateCategoryDtoToCategory(this UpdateCategoryDto category)
        {
            return new Category
            {
                Name = category.Name,
            };
        }
    }
}
