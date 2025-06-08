using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models;
using complaints_back.models.Complaints;
using complaints_back.Validations;

namespace complaints_back.Services.ComplaintService
{
    public class ComplaintService : IComplainService
    {
        public async Task<ServiceResponse<Complaint>> SubmitComplaint(AddComplaintDto complain)
        {
            var complaintValidationResult = new ComplaintUserValidation().Validate(complain);

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

            return new ServiceResponse<Complaint>
            {
                Success = false,
                Message = string.Join(", ", complaintValidationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = 400,
                Data = null
            };


        }
    }
}