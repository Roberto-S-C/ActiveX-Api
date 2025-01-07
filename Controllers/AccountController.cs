using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Account;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ActiveX_Api.Controllers
{
    [Route("api/account/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApiUser> _signInManager;

        public AccountController(
            AppDbContext context,
            IConfiguration configuration,
            UserManager<ApiUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApiUser> signInManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult> register([FromBody] RegisterUserDto userDto)
        {
            try
            {
                var newUser = new ApiUser();
                newUser.UserName = userDto.UserName;
                newUser.Email = userDto.Email;
                var result = await _userManager.CreateAsync(newUser, userDto.Password);

                if (result.Succeeded) {
                    return Ok($"User {userDto.UserName} created.");
                }
                else
                {
                    var error = result.Errors.Select(e => e.Description);
                    return BadRequest(error);
                }
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> login([FromBody] LoginUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userDto.UserName);

                if (user == null)
                {
                    return BadRequest("Invalid Credentials");
                }
                else
                {
                    bool validPassword = await _userManager.CheckPasswordAsync(user, userDto.Password);
                    if (!validPassword)
                    {
                        return BadRequest("Invalid Credentials");
                    }
                    else
                    {
                        var signinCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(
                                    _configuration["JWT:SigningKey"]))
                        , SecurityAlgorithms.HmacSha256);

                        var claims = new List<Claim>();
                        claims.Add(new Claim(
                            "UserId", user.Id));
                        claims.Add(new Claim(
                            ClaimTypes.Name, user.UserName));

                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var jwtObject = new JwtSecurityToken(
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(7),
                            signingCredentials: signinCredentials);

                        var jwtString = new JwtSecurityTokenHandler()
                            .WriteToken(jwtObject);

                        return Ok(jwtString);
                    }

                }


            }
            catch (Exception e)  
            {
                    return BadRequest($"Exception: {e.Message}"); 
            }
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> addAdministrator([FromBody] AddAdminDto userDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userDto.UserName);
                if (user == null) return NotFound("Username not found.");

                if(!await _roleManager.RoleExistsAsync(RoleNames.Administrator))
                {
                    await _roleManager.CreateAsync(new IdentityRole(RoleNames.Administrator));
                }

                var result = await _userManager.AddToRoleAsync(user, RoleNames.Administrator);
                if(result.Succeeded) return Ok($"User {user.UserName} added to Admin Role.");

                return BadRequest($"Assigning admin role to {user.UserName} failed.");
            }
            catch(Exception e) {
                return BadRequest(e.Message);
            }
        } 
    }
}
