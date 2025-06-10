using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models;

namespace complaints_back.Services.CategoriesService
{
    public interface ICategoriesService
    {
        ServiceResponse<List<Categories>> GetCategories();

        
    }
}