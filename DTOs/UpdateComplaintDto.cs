using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.DTOs
{
    public class UpdateComplaintDto
    {
        public int ComplaintId { get; set; }
        public string NewMessage { get; set; } = string.Empty;
        public ComplainStatus NewStatus { get; set; }
    }
}