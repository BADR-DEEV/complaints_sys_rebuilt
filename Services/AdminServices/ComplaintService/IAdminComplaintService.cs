using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Complaints;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.Services.AdminUserService
{
    public interface IAdminComplaintService
    {
        Task<ServiceResponseAdmin<List<Complaint>>> GetAllComplaints(int pageNumber, int pageSize);
        Task<ServiceResponseAdmin<Complaint>> UpdateComplaint(UpdateComplaintDto updateDto);
        Task<ServiceResponseAdmin<string>> DeleteComplaint(int complaintId);

    }
}