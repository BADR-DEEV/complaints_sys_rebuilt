using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using complaints_back.models.Users;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.models.Complaints
{
    public class Complaint
    {
        public int Id { get; set; }

        public string ComplainTitle { get; set; } = string.Empty;
        public string ComplainDescription { get; set; } = string.Empty;
        public string ComplaintMessage { get; set; } = string.Empty;
        public DateTime ComplainDateTime { get; set; } = DateTime.Now;
        public ComplainStatus ComplainStatus { get; set; } = ComplainStatus.Open;
        public string? FileName { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFormFile? Image { get; set; }

        //forign key category id
        public int CategoriesId { get; set; }
        public Categories Categories { get; set; }

        //forign key user id
        
        public string UserId { get; set; }
        public User User { get; set; }

    }
}