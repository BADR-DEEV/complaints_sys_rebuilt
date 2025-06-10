using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.models.Complaints;
using complaints_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    public class UserComplaintsController : Controller
    {

        private readonly IComplainService _complaintService;
        private readonly IConfiguration _Configuration;
        public UserComplaintsController(IComplainService authenticateUserService, IConfiguration configuration)
        {
            _complaintService = authenticateUserService;
            _Configuration = configuration;





        }



        [HttpGet]
        [Route("GetComplaints")]

        public async Task<ActionResult<ServiceResponse<List<Complaint>>>> GetAllComplains()
        {


            Helpers<List<Complaint>> helper = new();
            return helper.HandleResponse(await _complaintService.GetComplaints());
        }



        // This controller will handle user complaints
        // Add your action methods here

        [HttpGet]
        public IActionResult GetMyComplaints()
        {
            // Logic to get user complaints
            return Ok(new { message = "List of user complaints" });
        }

        [HttpPost("CreateComplaint")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult<ServiceResponse<Complaint>>> CreateComplaint([FromForm] AddComplaintDto complaint)
        {
            // Logic to create a new user complaint
            Helpers<Complaint> helper = new();
            return helper.HandleResponse(await _complaintService.SubmitComplaint(complaint));
        }

        // Add more methods as needed for handling user complaints

    }
}