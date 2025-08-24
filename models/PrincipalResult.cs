using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace complaints_back.models
{
    public class PrincipalResult
    {
        public ClaimsPrincipal? Principal { get; set; }
        public string? ErrorMessage { get; set; }
    }
}