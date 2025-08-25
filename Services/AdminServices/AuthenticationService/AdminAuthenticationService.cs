using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Users;
using complaints_back.Validations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;

namespace complaints_back.Services.AdminAuthenticationService
{
    public class AdminAuthenticationService : IAdminAuthenticationService
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailSenderService _emailSender;
        private readonly IUrlHelper _urlHelper;

        public AdminAuthenticationService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            IMapper mapper,
                IActionContextAccessor actionContextAccessor,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, EmailSenderService emailSender, IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }


        public async Task<ServiceResponse<string>> LogoutUserDashboard()
        {
            // Sign out from ASP.NET Identity
            await _signInManager.SignOutAsync();

            // Expire the cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // true in production
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = System.DateTime.UtcNow.AddDays(-1) // expire in past
            };

            _httpContextAccessor.HttpContext!.Response.Cookies.Append("accessToken", "", cookieOptions);
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", "", cookieOptions);

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Logged out successfully",
                StatusCode = 200,
                Data = null
            };
        }
    
        public async Task<ServiceResponse<UserResponseDto>> LoginUserDashboard(UserLoginDto authenticateUser)
        {
            var validationResult = new UserLoginValidation().Validate(authenticateUser);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    StatusCode = 400,
                    Data = null
                };
            }

            var user = await _userManager.FindByEmailAsync(authenticateUser.Email);
            if (user == null)
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "Invalid email or email not confirmed",
                    StatusCode = 400,
                    Data = null
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, authenticateUser.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "Invalid password",
                    StatusCode = 400,
                    Data = null
                };
            }

            if (user.Role != "Admin" && user.Role != "SuperAdmin")
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "You Are Not an Admin",
                    StatusCode = 400,
                    Data = null
                };

            }

            // Generate JWT token for authenticated user
            var jwtToken = await GenerateJwtToken(user);
            string refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(3); // or longer

            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,               // true in production, false in local dev
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(3)
            };

            _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            _httpContextAccessor.HttpContext!.Response.Cookies.Append("accessToken", jwtToken, cookieOptions);

            // Return it in the response
            return new ServiceResponse<UserResponseDto>
            {
                Success = true,
                Message = "Login successful",
                StatusCode = 200,
                Data = new UserResponseDto
                {
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    // AccessToken = jwtToken,
                    // RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
                }
            };

        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.DisplayName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("IsAdmin", (user.Role == "Admin" || user.Role == "SuperAdmin").ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Email", user.Email ?? string.Empty),
    };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var Security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value ?? "some default key"));
            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                expires: DateTime.UtcNow.AddDays(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(Security, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}