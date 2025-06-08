using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.DTOs;
using complaints_back.models.Users;

namespace complaints_back.models
{
    public class TokenApiModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

}