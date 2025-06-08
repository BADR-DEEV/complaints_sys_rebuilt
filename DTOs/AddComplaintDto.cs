using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models;

namespace complaints_back.DTOs
{
    public class AddComplaintDto
    {

        public string title { get; set; }
        public string description { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }



    }
}