using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Complaints;

namespace complaints_back.Services
{
    public interface IComplainService
    {
        Task<ServiceResponse<List<Complaint>>> GetComplaints();
        // Task<ServiceResponse<Complain>> GetComplaint(int id);
        Task<ServiceResponse<Complaint>> SubmitComplaint(AddComplaintDto complain);
        // Task<ServiceResponse<Complain>> UpdateComplaint(UpdateComplainDto complain);
        // Task<ServiceResponse<Complain>> DeleteComplaint(int id);  
    }
}