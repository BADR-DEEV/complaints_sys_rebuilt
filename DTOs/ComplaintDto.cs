using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static complaints_back.models.Complaints.ComplaintStatus;

namespace complaints_back.DTOs
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string ComplainTitle { get; set; } = string.Empty;
        public string ComplainDescription { get; set; } = string.Empty;
        public string ComplaintMessage { get; set; } = string.Empty;

        public ComplainStatus ComplainStatus { get; set; }
        public string? FileName { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFormFile? Image { get; set; }

        //forign key category id
        public int CategoriesId { get; set; }
    }
}