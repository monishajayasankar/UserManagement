using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.API.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManagementContext context;
        private readonly IConfiguration configuration;

        public UserController(UserManager<AppUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(RegisterRequestDTO dto)
        {
            AppUser user = new AppUser
            {

                FullName = dto.Fullname,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            IdentityResult result = await userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                bool IsRolePresent = await roleManager.RoleExistsAsync("Employee");
                result = await userManager.AddToRoleAsync(user, "Employee");
                if (result.Succeeded)
                {
                    return StatusCode(201);
                }
            }
            return BadRequest(result.Errors);

        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(LoginRequestDTO dto)
        {
            AppUser user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return BadRequest("Invalid username/password");
            bool IsValidPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!IsValidPassword)
                return BadRequest("Invalid username/password");

            string key = configuration["JWT:key"];
            string issuer = configuration["JWT:issuer"];
            string audience = configuration["JWT:audience"];

            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            DateTime expires = DateTime.Now.AddMinutes(30); //Expiration Time for token 
            SecurityKey securityKey = new SymmetricSecurityKey(keyBytes);

            var userClaims = await userManager.GetClaimsAsync(user);
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));

            var userRoles = await userManager.GetRolesAsync(user);
            var role = userRoles.First();
            userClaims.Add(new Claim(ClaimTypes.Role, role));

            SigningCredentials credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: userClaims,
                signingCredentials: credentails,
                expires: expires);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string jwt = handler.WriteToken(token);

            var response = new LoginResponseDTO() { UserName = user.UserName,FullName=user.FullName ,Role = role, Token = jwt };
            return Ok(response);
        }



    }
}
