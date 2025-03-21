using ActiveX_Api.Dto.OrderItem;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class OrderItemMapper
    {
        public static GetOrderItemDto FromOrderItemToGetOrderItemDto(this OrderItem orderItem)
        {
            return new GetOrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity
            };
        }   
        public static OrderItem FromCreateOrderItemDtoOrderItem (this CreateOrderItemDto orderItemDto)
        {
            return new OrderItem
            {
                OrderId = orderItemDto.OrderId,
                ProductId = orderItemDto.ProductId,
                Price = orderItemDto.Price,
                Quantity = orderItemDto.Quantity
            };
        }
    }
}
