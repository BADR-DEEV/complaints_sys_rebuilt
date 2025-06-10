using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Data;
using complaints_back.models;
using Microsoft.EntityFrameworkCore;

namespace complaints_back.Services.CategoriesService
{
    public class CategoriesService : ICategoriesService
    {

        private readonly IConfiguration _Configuration;
        private readonly DataContext _Context;
        public CategoriesService(DataContext dataContext, IConfiguration configuration)
        {
            _Context = dataContext;
            _Configuration = configuration;
        }


        public ServiceResponse<List<Categories>> GetCategories()
        {
            ServiceResponse<List<Categories>> serviceResponse = new ServiceResponse<List<Categories>>();

            try
            {
                // var x = await _userManager.FindByEmailAsync(GetUserId()) ?? throw new Exception("User not found");

                serviceResponse.Data = _Context.Categories.ToList();

                if (serviceResponse.Data == null || serviceResponse.Data.Count == 0)
                {
                    serviceResponse.Message = "No Categories were found, the admin had delete them all";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = 404;
                }

                else
                {
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = 200;

                }

            }
            catch (Exception e)
            {
                serviceResponse.Message = "Server Error : " + e.Message;
                serviceResponse.Success = false;
                serviceResponse.StatusCode = 500;
            }
            return serviceResponse;
        }





    }
}