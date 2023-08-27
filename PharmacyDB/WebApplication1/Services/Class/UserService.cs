using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PharmacyInfrastructure;
using PharmacyInfrastructure.View;
using PharmacyWeb.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PharmacyWeb.Services.Class
{
    public class UserService : IUserService
    {
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        public UserService(UserManager<IdentityUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<UserManagerResponse> RegisterUser(RegisterViewModel model)
        {
            if(model == null)
                throw new NullReferenceException("Register Model is null");
            if(model.Password != model.ConfirmedPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirmed Password doesn't match password",
                    IsSuccess = false,
                };
            }
            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(identityUser,model.Password);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "User created successfully",
                    IsSuccess = true
                };
            }
            return new UserManagerResponse
            {
                Message = "User did not created",
                IsSuccess = false,
                Errors = result.Errors.Select(x => x.Description)
            };
        }
        public async Task<UserManagerResponse> LoginUser(LoginViewModel model)
        {
            var user= await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Wrong Email Address",
                    IsSuccess=false,
                };
            }
            var result= await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password",
                    IsSuccess = false,
                };
            }
            var claims = new[]
            {
                new Claim("Email",model.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials : new SigningCredentials(key,SecurityAlgorithms.HmacSha256)
                ); ;
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse
            {
                Message = tokenString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

    }
}
