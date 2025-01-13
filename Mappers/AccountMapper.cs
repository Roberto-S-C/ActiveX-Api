using ActiveX_Api.Dto.Account;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class AccountMapper
    {
        public static UserDto FromApiUserToUserDto (this ApiUser user) 
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };
        }
    }
}
