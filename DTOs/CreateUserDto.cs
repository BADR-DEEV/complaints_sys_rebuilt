using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace complaints_back.DTOs
{
    public class CreateUserDto
    {
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = "User";

    }
}