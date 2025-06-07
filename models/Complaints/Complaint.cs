using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.models.Complaints
{
    public class Complaint
    {
        public int Id { get; set; }

        public string ComplainTitle { get; set; } = string.Empty;
        public string ComplainDescription { get; set; } = string.Empty;
        public DateTime ComplainDateTime { get; set; } = DateTime.Now;
        public ComplainStatus ComplainStatus { get; set; } = ComplainStatus.Open;


        // public User? PersonUser { get; set; }

        // [ForeignKey("Category")]
        // public int CategoryId { get; set; }
        // public virtual Category? Category { get; set; }

    }
}