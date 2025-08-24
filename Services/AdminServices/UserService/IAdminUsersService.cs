using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Users;

namespace complaints_back.Services.AdminServices.AdminComplaintService
{
    public interface IAdminUsersService
    {
        Task<ServiceResponseAdmin<List<User>>> GetAllUsers(int pageNumber, int pageSize);
        Task<ServiceResponseAdmin<User>> CreateUser(CreateUserDto createUserDto);
        Task<ServiceResponseAdmin<string>> DeleteUser(string userId);

    }
}