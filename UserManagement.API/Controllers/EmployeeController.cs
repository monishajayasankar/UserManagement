using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManagementContext context;
        private readonly IConfiguration configuration;
        public EmployeeController(UserManager<AppUser> userManager,
               RoleManager<IdentityRole> roleManager,
               IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEmployees()
        {
            var dtos = from User in userManager.Users
                       select new EmployeeDto
                       {
                           Id = User.Id,
                           Fullname = User.FullName,
                           UserName = User.UserName,
                           Email = User.Email,
                           PhoneNumber = User.PhoneNumber
                       };

            return Ok(dtos);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var dtos =  new EmployeeDto
                       {
                           Id = user.Id,
                           Fullname = user.FullName,
                           UserName = user.UserName,
                           Email = user.Email,
                           PhoneNumber = user.PhoneNumber
                       };

            return Ok(dtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(string id,[FromBody] EmployeeDto dto)
        {
            AppUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            else
            {
                user.FullName = dto.Fullname;
                user.Email = dto.Email;
                user.UserName = dto.UserName;
                user.PhoneNumber = dto.PhoneNumber;

                var result = await userManager.UpdateAsync(user);

            }
            return Ok("Employee Updated");

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            var result = await userManager.DeleteAsync(user);

            
            return Ok("Employee "+user.FullName+" Deleted");

        }





    }
}
