using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.models.Users;
using complaints_back.Services;
using complaints_back.Services.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace complaints_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _AuthenticateUserService;
        private readonly UserManager<User> _userManager;
        private readonly EmailSenderService _emailSender;
        private readonly IConfiguration _Configuration;
        public AuthController(IAuthenticationService authenticateUserService, UserManager<User> userManager, IConfiguration configuration)
        {
            _AuthenticateUserService = authenticateUserService;
            _userManager = userManager;
            _Configuration = configuration;





        }
        [HttpPost("RegisterAppUser")]
        public async Task<ActionResult<ServiceResponse<UserResponseDto>>> Register([FromBody] UserRegisterDto user)
        {

            Helpers<UserResponseDto> helper = new();
            return helper.HandleResponse(await _AuthenticateUserService.RegisterUser(user));


        }

        [HttpPost("LoginAppUser")]

        public async Task<ActionResult<ServiceResponse<UserResponseDto>>> Login([FromBody] UserLoginDto user)
        {
            Helpers<UserResponseDto> helper = new();
            return helper.HandleResponse(await _AuthenticateUserService.LoginUser(user));
        }

        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            string decodedToken = WebUtility.UrlDecode(token);
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Email or token is missing.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!confirmResult.Succeeded)
            {
                return BadRequest("Email confirmation failed.");
            }

            // Unlock user to allow login
            user.LockoutEnd = null;
            user.LockoutEnabled = false;
            await _userManager.UpdateAsync(user);

            // Generate JWT token now for authenticated user (optional if needed in app)
            var jwtToken = await _AuthenticateUserService.GenerateJwtToken(user);

            // Build the deep link with token (you may want to URL encode the token again)
            var appDeepLink = $"complaintsapp://confirm-email?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}&jwt={WebUtility.UrlEncode(jwtToken)}";

            // Redirect to the Flutter app via deep link
            return Redirect(appDeepLink);
        }
    }
}