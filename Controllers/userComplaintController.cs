using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class userComplaints : ControllerBase
    {
        // This controller will handle user complaints
        // Add your action methods here

        [HttpGet]
        public IActionResult GetMyComplaints()
        {
            // Logic to get user complaints
            return Ok(new { message = "List of user complaints" });
        }

        [HttpPost]
        public IActionResult CreateComplaint([FromBody] string complaint)
        {
            // Logic to create a new user complaint
            return CreatedAtAction(nameof(GetMyComplaints), new { message = "Complaint created successfully" });
        }

        // Add more methods as needed for handling user complaints
        
    }
}