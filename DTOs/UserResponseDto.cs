using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace complaints_back.DTOs
{
    public class UserResponseDto
    {
        public required string DisplayName { get; set; }
        public required string Email { get; set; }

        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccessToken { get; set; } = null;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}