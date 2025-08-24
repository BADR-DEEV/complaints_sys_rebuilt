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
using complaints_back.Services.AdminServices.AdminComplaintService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.Services.AdminUserService
{
    public class AdminComplaintService : IAdminComplaintService
    {
        private readonly DataContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminComplaintService(

            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, DataContext context,
             UserManager<User> userManager, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {

            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

        }





        public async Task<ServiceResponseAdmin<List<Complaint>>> GetAllComplaints(int pageNumber = 1, int pageSize = 10)
        {
            var serviceResponse = new ServiceResponseAdmin<List<Complaint>>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    serviceResponse.Message = "User not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 400;
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
                var totalComplaints = await _context.Complaints.CountAsync();

                // Apply pagination
                var complaints = await _context.Complaints
                    .OrderByDescending(c => c.Id) // optional: keep newest first
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                serviceResponse.Data = complaints;

                serviceResponse.Message = complaints.Count == 0
                    ? "No Complaints were found"
                    : $"{complaints.Count} Complaints retrieved";

                serviceResponse.Success = true;
                serviceResponse.StatusCode = 200;

                // You can include pagination info in response metadata
                serviceResponse.TotalCount = complaints.Count;
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

        public async Task<ServiceResponseAdmin<Complaint>> UpdateComplaint(UpdateComplaintDto updateDto)
        {
            var serviceResponse = new ServiceResponseAdmin<Complaint>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    serviceResponse.Message = "User not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 400;
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

                var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == updateDto.ComplaintId);

                if (complaint == null)
                {
                    serviceResponse.Message = "Complaint not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                    return serviceResponse;
                }

                // Update complaint status and message
                complaint.ComplainStatus = updateDto.NewStatus;
                complaint.ComplaintMessage = updateDto.NewMessage;

                _context.Complaints.Update(complaint);
                await _context.SaveChangesAsync();

                serviceResponse.Data = complaint;
                serviceResponse.Message = "Complaint updated successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = 200;
            }
            catch (Exception e)
            {
                serviceResponse.Message = "Server Error: " + e.Message;
                serviceResponse.Success = false;
                serviceResponse.StatusCode = 500;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponseAdmin<string>> DeleteComplaint(int complaintId)
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
                    serviceResponse.Message = "Forbidden: Only Admins can delete complaints.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 403;
                    return serviceResponse;
                }

                // 3. Find the complaint
                var complaint = await _context.Complaints.FindAsync(complaintId);
                if (complaint == null)
                {
                    serviceResponse.Message = $"Complaint with ID {complaintId} not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                    return serviceResponse;
                }

                // Optional: if you want to block deleting closed/critical complaints
                // if (complaint.Status == ComplainStatus.Closed) {
                //     serviceResponse.Message = "Closed complaints cannot be deleted.";
                //     serviceResponse.Success = false;
                //     serviceResponse.StatusCode = 400;
                //     return serviceResponse;
                // }

                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();

                serviceResponse.Data = $"Complaint (ID: {complaint.Id}) was deleted successfully.";
                serviceResponse.Message = "Complaint deleted successfully.";
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


    }
}