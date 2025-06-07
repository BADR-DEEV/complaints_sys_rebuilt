using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
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

namespace complaints_back.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailSenderService _emailSender;
        private readonly IUrlHelper _urlHelper;

        public AuthenticationService(
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


        public async Task<ServiceResponse<UserResponseDto>> RegisterUser(UserRegisterDto authenticateUser)
        {
            var validationResult = new UserRegisterValidation().Validate(authenticateUser);
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

            var existingUser = await _userManager.FindByEmailAsync(authenticateUser.Email);
            if (existingUser != null)
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "User already exists",
                    StatusCode = 400,
                    Data = null
                };
            }

            var user = new User
            {
                DisplayName = authenticateUser.DisplayName,
                UserName = Guid.NewGuid().ToString(),  // Generate a unique internal username
                Email = authenticateUser.Email,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                LockoutEnabled = true, // Prevent login before email confirmed



            };

            var createResult = await _userManager.CreateAsync(user, authenticateUser.Password);
            if (!createResult.Succeeded)
            {
                return new ServiceResponse<UserResponseDto>
                {
                    Success = false,
                    Message = string.Join(", ", createResult.Errors.Select(e => e.Description)),
                    StatusCode = 400,
                    Data = null
                };
            }

            var link = await SendConfirmationEmail(user.Email, user);

            return new ServiceResponse<UserResponseDto>
            {
                Success = true,
                Message = $"Email sent successfully. Please confirm your email to complete registration: {link}",
                StatusCode = 200,
                Data = user == null ? null : _mapper.Map<UserResponseDto>(user)
            };
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.DisplayName),

        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Email", user.Email ?? string.Empty),





    };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var Security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value ?? "some default key"));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(Security, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private async Task<string> SendConfirmationEmail(string email, User user)
        {
            // user.AccessToken = jwtToken;
            // Generate the email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // var confirmationLink = _urlHelper.Action("ConfirmEmail", "Auth",
            //     new { email = user.Email, token = encodedToken }, protocol: _httpContextAccessor.HttpContext?.Request.Scheme);
            // var frontendBaseUrl = "complaintsapp://confirm-email";
            // var query = $"?email={WebUtility.UrlEncode(user.Email)}&token={WebUtility.UrlEncode(encodedToken)}";
            // var confirmationLink = $"{frontendBaseUrl}{query}";


            var confirmationLink = _urlHelper.Action("ConfirmEmail", "Auth",
                new { email = user.Email, token = encodedToken },
                protocol: _httpContextAccessor.HttpContext?.Request.Scheme);


            // // Build the confirmation callback URL
            // var confirmationLink = _urlHelper.Action("ConfirmEmail", "Auth",
            //     new { email = user.Email, emailToken = token }, protocol: _httpContextAccessor.HttpContext?.Request.Scheme);

            // Encode the link to prevent XSS and other injection attacks
            var safeLink = HtmlEncoder.Default.Encode(confirmationLink);

            // Craft a more polished email subject
            var subject = "Welcome to شكاوي ليبيا ! Please Confirm Your Email";

            // Create a professional HTML body
            // Customize inline styles, text, and branding as needed
            var messageBody = $@"
        <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;"">
            <p>Hi {user.DisplayName},</p>

            <p>Thank you for creating an account at <strong>نظام شكاوي</strong>.
            To start using all of our features, please confirm your email address by clicking the button below:</p>

            <p>
                <a href=""{safeLink}"" 
                   style=""background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;
                          font-weight:bold;border-radius:5px;display:inline-block;"">
                    Confirm Email
                </a>
            </p>

           

         

            <p>If you did not sign up for this account, please ignore this email.</p>
{safeLink}
            <p>Thanks,<br />

            
            Badr's Team</p>
        </div>
    ";

            //Send the Confirmation Email to the User Email Id
            await _emailSender.SendEmailAsync(email, subject, messageBody, true);
            return confirmationLink;
        }

        public async Task<ServiceResponse<UserResponseDto>> LoginUser(UserLoginDto authenticateUser)
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
            if (user == null || !user.EmailConfirmed)
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

            // Generate JWT token for authenticated user
            var jwtToken = await GenerateJwtToken(user);

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
                    AccessToken = jwtToken
                }
            };
        }

    }
}