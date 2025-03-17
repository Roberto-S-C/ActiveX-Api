using ActiveX_Api.Models;
using ActiveX_Api.Dto.Address;

namespace ActiveX_Api.Mappers
{
    public static class AddressMapper
    {
        public static Address FromCreateAddresDtoToAddress(this CreateAddressDto address)
        {
            return new Address
            {
               FullName = address.FullName,
               Street = address.Street,
               Number = address.Number,
               City = address.City,
               State = address.State,
               PostalCode = address.PostalCode,
               Country = address.Country,
               Phone = address.Phone,
            };
        }

        public static AddressListDto FromAddressToAddressListDto(this Address address)
        {
            return new AddressListDto
            {
                Id = address.Id,
                FullName = address.FullName,
                Street = address.Street,
                Number = address.Number,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                Phone = address.Phone,
            };
        }
    }
}
