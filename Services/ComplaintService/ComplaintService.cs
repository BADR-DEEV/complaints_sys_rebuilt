using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using complaints_back.Data;
using complaints_back.DTOs;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.models.Complaints;
using complaints_back.models.Users;
using complaints_back.Validations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace complaints_back.Services.ComplaintService
{
    public class ComplaintService : IComplainService
    {
        private readonly DataContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintService(

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

        public async Task<ServiceResponse<List<Complaint>>> GetComplaints()
        {

            ServiceResponse<List<Complaint>> serviceResponse = new ServiceResponse<List<Complaint>>();

            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // var x = await _userManager.FindByEmailAsync(GetUserId()) ?? throw new Exception("User not found");
                if (userId == null)
                {
                    serviceResponse.Message = "User not found, (log out)";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 400;

                }
                serviceResponse.Data = await _context.Complaints
                    .Include(c => c.Categories)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (serviceResponse.Data == null)
                {
                    serviceResponse.Message = "No Complaints were found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                }

                else
                {
                    serviceResponse.Message = serviceResponse.Data.Count == 0 ? "No Complaints were found" : $"{serviceResponse.Data.Count}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = 200;
                }

            }
            catch (Exception e)
            {
                serviceResponse.Message = "Server Error : " + e.Message;
                serviceResponse.Success = false;
                serviceResponse.StatusCode = 500;
            }
            return serviceResponse;
        }







        public async Task<ServiceResponse<Complaint>> SubmitComplaint(AddComplaintDto addComplaint)
        {
            var complaintValidationResult = new ComplaintUserValidation().Validate(addComplaint);
            Complaint complaint = _mapper.Map<Complaint>(addComplaint);

            if (!complaintValidationResult.IsValid)
            {
                return new ServiceResponse<Complaint>
                {
                    Success = false,
                    Message = string.Join(", ", complaintValidationResult.Errors.Select(e => e.ErrorMessage)),
                    StatusCode = 400,
                    Data = null
                };
            }

            try
            {


                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return new ServiceResponse<Complaint>
                    {
                        Success = false,
                        Data = null,
                        Message = "UnAuthorized, log out this user"
                    };

                }
                complaint.UserId = userId;
                complaint.Categories = await _context.Categories.FirstOrDefaultAsync(c => c.Id == addComplaint.CategoriesId);

                if (complaint.Categories == null)
                {
                    return new ServiceResponse<Complaint>
                    {
                        Success = false,
                        Data = null,
                        Message = "Category Not Found",
                        StatusCode = 400
                    };

                }
                string? uniqueFileName = null;

                if (addComplaint.Image != null)
                {
                    // Ensure the uploads directory exists
                    string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads", "complaints"); // Match with RequestPath configuration
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + addComplaint.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);


                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            //write to filestream
                            await addComplaint.Image.CopyToAsync(fileStream);
                        }

                        // Construct the public URL for the image
                        // This assumes your StaticFiles RequestPath is "/images" as configured in Program.cs
                        // and that "complaints" is a subfolder within your "Uploads" directory.
                        var imageUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}/images/complaints/{uniqueFileName}";
                        complaint.FileName = imageUrl;

                    }
                    catch (Exception ex)
                    {

                        return new ServiceResponse<Complaint>
                        {
                            Success = false,
                            Data = null,
                            Message = "error in uploading image",
                            StatusCode = 500,

                        };
                    }

                    _context.Complaints.Add(complaint);
                    await _context.SaveChangesAsync();

                    return new ServiceResponse<Complaint>
                    {
                        Success = true,
                        Data = complaint,
                        Message = "hhhhh",
                        StatusCode = 201,

                    };
                }
                else
                {
                    return new ServiceResponse<Complaint>
                    {
                        Success = true,
                        Data = complaint,
                        Message = "No Image was uploaded",
                        StatusCode = 201,

                    };
                }
            }
            catch (Exception e)
            {
                return new ServiceResponse<Complaint>
                {
                    Success = false,
                    Data = null,
                    Message = $"Unhandled Exception: {e}",
                    StatusCode = 500
                };

            }
        }
    }
}