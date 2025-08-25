using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.models.Complaints;
using complaints_back.Services;
using complaints_back.Services.AdminUserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Identity.Application", Roles = "Admin,SuperAdmin")]
    public class AdminComplaintController : ControllerBase
    {
        private readonly IAdminComplaintService _adminComplaintService;
        private readonly IConfiguration _Configuration;
        public AdminComplaintController(IAdminComplaintService adminComplaints, IConfiguration configuration)
        {
            _adminComplaintService = adminComplaints;
            _Configuration = configuration;

        }

        [HttpGet]
        [Route("GetAllComplaints")]

        public async Task<ActionResult<ServiceResponseAdmin<List<Complaint>>>> GetAllComplains(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {


            HelpersAdmin<List<Complaint>> helper = new();
            return helper.HandleResponse(await _adminComplaintService.GetAllComplaints(
                pageNumber, pageSize
            ));
        }


        [HttpPut]
        [Route("UpdateComplaint")]

        public async Task<ActionResult<ServiceResponseAdmin<Complaint>>> UpdateComplaint(
    [FromBody] UpdateComplaintDto updateDto
)
        {
            HelpersAdmin<Complaint> helper = new();
            return helper.HandleResponse(await _adminComplaintService.UpdateComplaint(
                updateDto
            ));
        }


        [HttpDelete]
        [Route("DeleteComplaint")]

        public async Task<ActionResult<ServiceResponseAdmin<string>>> DeleteComplaint(
           [FromQuery] int complaintId
        )
        {


            HelpersAdmin<string> helper = new();
            return helper.HandleResponse(await _adminComplaintService.DeleteComplaint(
                complaintId

            ));
        }


    }
}