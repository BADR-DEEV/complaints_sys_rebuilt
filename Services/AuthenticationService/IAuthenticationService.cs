using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Users;

namespace complaints_back.Services.AuthenticationService
{
    public interface IAuthenticationService
    {

        Task<ServiceResponse<UserResponseDto>> RegisterUser(UserRegisterDto authenticateUser);
        Task<string> GenerateJwtToken(User user);
        Task<ServiceResponse<UserResponseDto>> LoginUser(UserLoginDto authenticateUser);
          // Task<ServiceResponse<User>> RegisterUser(UserRegisterDto authenticateUser, string role);
        // Task<ServiceResponse<User>> LoginUser(UserLoginDto authenticateUser);
        // Task<ServiceResponse<User>> Logout();
        // string RefreshToken(User authenticateUser);
    }
}