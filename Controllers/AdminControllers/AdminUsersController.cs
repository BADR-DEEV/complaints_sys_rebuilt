using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Users;
using complaints_back.Services.AdminServices.AdminComplaintService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AdminUsersController : ControllerBase
    {

        private readonly IAdminUsersService _adminUsersService;
        private readonly IConfiguration _Configuration;
        public AdminUsersController(IAdminUsersService adminUsers, IConfiguration configuration)
        {
            _adminUsersService = adminUsers;
            _Configuration = configuration;

        }
        [HttpGet]
        [Route("GetAllUsers")]

        public async Task<ActionResult<ServiceResponseAdmin<List<UserDto>>>> GetAllComplains(
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10)
        {


            HelpersAdmin<List<UserDto>> helper = new();
            return helper.HandleResponse(await _adminUsersService.GetAllUsers(
                pageNumber, pageSize
            ));
        }



        [HttpDelete]
        [Route("DeleteUser")]

        public async Task<ActionResult<ServiceResponseAdmin<string>>> DeleteUser(
            [FromQuery] string userId
         )
        {


            HelpersAdmin<string> helper = new();
            return helper.HandleResponse(await _adminUsersService.DeleteUser(
                userId

            ));
        }

        [HttpPost]
        [Route("CreateUser")]

        public async Task<ActionResult<ServiceResponseAdmin<User>>> CreateUser(
         [FromBody] CreateUserDto userDto
       )
        {


            HelpersAdmin<User> helper = new();
            return helper.HandleResponse(await _adminUsersService.CreateUser
            (
                userDto

            ));
        }
    }
}