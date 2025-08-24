using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models.Complaints;
using Microsoft.AspNetCore.Identity;

namespace complaints_back.models.Users
{
    public class User : IdentityUser
    {
        public required string DisplayName { get; set; }

        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();



    }
}