using ActiveX_Api.Dto.Order;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class OrderMapper
    {
        public static Order FromCreateOrderDtoToOrder(this CreateOrderDto createOrderDto)
        {
            return new Order
            {
                StripeSessionId = createOrderDto.StripeSessionId,
                Amount = createOrderDto.Amount,
                Status = createOrderDto.Status,
                AddressId = createOrderDto.AddressId
            };
        }
    }
}
