using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace complaints_back.models
{
    public class Categories
    {
        public int Id { get; set; }                     // Unique ID for the category
        public string Name { get; set; }        // Name of the category
        public string AR_Name { get; set; }
        public string AR_Des { get; set; }
        public string Description { get; set; }        // Name of the category


    }
}