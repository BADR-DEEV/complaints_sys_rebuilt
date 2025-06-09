using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models;

namespace complaints_back.DTOs
{
    public class AddComplaintDto
    {

        public string ComplainTitle { get; set; }
        public string ComplainDescription { get; set; }
        public int CategoriesId { get; set; }
        public IFormFile? Image { get; set; }



    }
}