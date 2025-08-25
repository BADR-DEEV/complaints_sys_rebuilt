using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using complaints_back.Data;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Complaints;
using complaints_back.models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace complaints_back.Services.AdminServices.AdminComplaintService
{
    public class AdminUsersService : IAdminUsersService
    {
        private readonly DataContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUsersService(

 RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, DataContext context,
             UserManager<User> userManager, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {

            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

        }

        public async Task<ServiceResponseAdmin<string>> DeleteUser(string userId)
        {
            var serviceResponse = new ServiceResponseAdmin<string>();

            try
            {
                // 1. Check if logged in
                var currentUserId = _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    serviceResponse.Message = "Unauthorized: Please log in again.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 401;
                    return serviceResponse;
                }

                // 2. Check if user has required role
                var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin" && userRole != "SuperAdmin")
                {
                    serviceResponse.Message = "Forbidden: Only Admins can delete users.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 403;
                    return serviceResponse;
                }

                // 3. Find the user
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    serviceResponse.Message = $"User with ID {userId} not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                    return serviceResponse;
                }

                if (user.Id == currentUserId)
                {
                    serviceResponse.Message = $"You can't delete this user since it's you (silly).";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 400;
                    return serviceResponse;

                }
                if (user.Role == "SuperAdmin")
                {
                    serviceResponse.Message = "You can't delete the super admin user, this should belong to the owner";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 400;
                    return serviceResponse;

                }


                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                serviceResponse.Data = $"User '{user.DisplayName}' (ID: {user.Id}) was deleted successfully.";
                serviceResponse.Message = "User deleted successfully.";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = 200;

                // For delete, pagination is irrelevant
                serviceResponse.TotalCount = 1;
                serviceResponse.PageNumber = 1;
                serviceResponse.PageSize = 1;

                return serviceResponse;
            }
            catch (Exception e)
            {
                serviceResponse.Message = "Server error: " + e.Message;
                serviceResponse.Success = false;
                serviceResponse.StatusCode = 500;
                return serviceResponse;
            }
        }




        public async Task<ServiceResponseAdmin<List<UserDto>>> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var serviceResponse = new ServiceResponseAdmin<List<UserDto>>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    serviceResponse.Message = "User not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                    return serviceResponse;
                }

                var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "SuperAdmin")
                {
                    serviceResponse.Message = "User not an Admin";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 403;
                    return serviceResponse;
                }

                // Count total complaints
                // var totalUsers = await _context.Users.CountAsync();

                // // Apply pagination
                // var users = await _context.Users
                //     .OrderByDescending(c => c.Id) // optional: keep newest first
                //     .Skip((pageNumber - 1) * pageSize)
                //     .Take(pageSize)
                //     .ToListAsync();



                var totalUsers = await _context.Users.CountAsync();
                var users = await _context.Users
            .OrderByDescending(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                DisplayName = u.DisplayName,
                id = u.Id,
                Email = u.Email,
                Role = u.Role,
                Complaints = u.Complaints.Select(c => new ComplaintDto
                {
                    Id = c.Id,
                    ComplainTitle = c.ComplainTitle,
                    ComplainDescription = c.ComplainDescription,
                    ComplaintMessage = c.ComplaintMessage,
                    ComplainStatus = c.ComplainStatus,
                    FileName = c.FileName,
                    Image = c.Image,
                    CategoriesId = c.CategoriesId
                }).ToList()
            })
            .ToListAsync();

                serviceResponse.Data = users;

                serviceResponse.Message = totalUsers.ToString();

                serviceResponse.Success = true;
                serviceResponse.StatusCode = 200;

                // You can include pagination info in response metadata
                serviceResponse.TotalCount = totalUsers;
                serviceResponse.PageNumber = pageNumber;
                serviceResponse.PageSize = pageSize;

            }
            catch (Exception e)
            {
                serviceResponse.Message = "Server Error : " + e.Message;
                serviceResponse.Success = false;
                serviceResponse.StatusCode = 500;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponseAdmin<User>> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                // check if user exists already
                var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (existingUser != null)
                {
                    return new ServiceResponseAdmin<User>
                    {
                        Success = false,
                        Message = "User with this email already exists",
                        StatusCode = 400,
                        Data = null
                    };
                }

                // create user object
                var user = new User
                {
                    UserName = createUserDto.Email,
                    Email = createUserDto.Email,
                    DisplayName = createUserDto.DisplayName,
                    Role = createUserDto.Role,
                    CreatedAt = DateTime.UtcNow
                };

                // create user with password
                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (!result.Succeeded)
                {
                    return new ServiceResponseAdmin<User>
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description)),
                        StatusCode = 400,
                        Data = null
                    };
                }

                // assign role if provided
                if (!string.IsNullOrEmpty(createUserDto.Role))
                {
                    if (!await _roleManager.RoleExistsAsync(createUserDto.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(createUserDto.Role));
                    }
                    await _userManager.AddToRoleAsync(user, createUserDto.Role);
                }

                return new ServiceResponseAdmin<User>
                {
                    Success = true,
                    Message = "User created successfully",
                    StatusCode = 201,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseAdmin<User>
                {
                    Success = false,
                    Message = $"Unhandled Exception: {ex.Message}",
                    StatusCode = 500,
                    Data = null
                };
            }
        }


    }
}