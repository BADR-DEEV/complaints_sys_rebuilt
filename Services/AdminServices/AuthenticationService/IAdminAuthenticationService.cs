using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.models;

namespace complaints_back.Services.AdminAuthenticationService
{
    public interface IAdminAuthenticationService
    {

        Task<ServiceResponse<UserResponseDto>> LoginUserDashboard(UserLoginDto authenticateUser);
        Task<ServiceResponse<string>> LogoutUserDashboard();

    }
}