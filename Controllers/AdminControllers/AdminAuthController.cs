using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.models.Users;
using complaints_back.Services.AdminAuthenticationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuth : ControllerBase
    {
        private readonly IAdminAuthenticationService _AuthenticateUserService;
        private readonly UserManager<User> _userManager;
        // private readonly EmailSenderService _emailSender;
        private readonly IConfiguration _Configuration;
        public AdminAuth(IAdminAuthenticationService authenticateUserService, UserManager<User> userManager, IConfiguration configuration)
        {
            _AuthenticateUserService = authenticateUserService;
            _userManager = userManager;
            _Configuration = configuration;

        }
        [HttpPost("Login")]

        public async Task<ActionResult<ServiceResponse<UserResponseDto>>> Login([FromBody] UserLoginDto user)
        {
            Helpers<UserResponseDto> helper = new();
            return helper.HandleResponse(await _AuthenticateUserService.LoginUserDashboard(user));
        }

    }
}