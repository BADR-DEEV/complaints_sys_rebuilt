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
        public async Task<IActionResult> ConfirmEmailAsync(string email, string token)
        {
            var response = new ServiceResponse<UserResponseDto>();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                response.Success = false;
                response.Message = "Email or token is missing.";
                response.StatusCode = 400;
                return BadRequest(response);

            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                response.StatusCode = 404;
                return NotFound(response);

            }

            string decodedToken = WebUtility.UrlDecode(token);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!confirmResult.Succeeded)
            {
                response.Success = false;
                response.Message = "Email confirmation failed.";
                response.StatusCode = 400;
                return BadRequest(response);
            }

            user.LockoutEnd = null;
            user.LockoutEnabled = false;
            await _userManager.UpdateAsync(user);

            var jwtToken = await _AuthenticateUserService.GenerateJwtToken(user);

            response.Success = true;
            response.Message = "Email confirmed successfully.";
            response.StatusCode = 200;
            response.Data = new UserResponseDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                AccessToken = jwtToken
            };


            // Build the deep link with token (you may want to URL encode the token again)
            var appDeepLink = $"complaintsapp://confirm-email?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}&response={response.StatusCode}";

            // Redirect to the Flutter app via deep link
            return Redirect(appDeepLink);

        }




        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(TokenApiModel model)
        {
            var _getPrinc = new GetPrincipalFromExpiredToken();

            var principal = _getPrinc.GetPrincipal(model.AccessToken, _Configuration);

            var email = principal?.FindFirst(ClaimTypes.Email)?.Value;


            if (email == null)
            {
                return Unauthorized(new { message = "The email was not found from the provided token" });

            }
            else
            {
                var user = await _userManager.FindByEmailAsync(email);


                if (user == null || user.RefreshToken != model.RefreshToken)
                {

                    return Unauthorized(new { message = $"Invalid refresh token {user.Email}........ {user.RefreshToken}........ {user.RefreshTokenExpiryTime}" });
                }

                var newAccessToken = await _AuthenticateUserService.GenerateJwtToken(user);
                var newRefreshToken = _AuthenticateUserService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
        }



    }
}
