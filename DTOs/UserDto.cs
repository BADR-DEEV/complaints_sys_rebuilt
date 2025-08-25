using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace complaints_back.DTOs
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<ComplaintDto> Complaints { get; set; }
    }

  

}