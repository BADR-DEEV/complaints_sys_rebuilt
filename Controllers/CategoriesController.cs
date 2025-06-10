using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.Helpers;
using complaints_back.models;
using complaints_back.Services.CategoriesService;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {


        private readonly ICategoriesService _CategoriesService;

        private readonly IConfiguration _Configuration;
        public CategoriesController(ICategoriesService CategoriesService, IConfiguration configuration)
        {
            _CategoriesService = CategoriesService;
            _Configuration = configuration;

        }


        [HttpGet]
        [Route("GetCategories")]

        public ActionResult<ServiceResponse<List<Categories>>> GetAllCategories()
        {


            Helpers<List<Categories>> helper = new();
            return helper.HandleResponse(_CategoriesService.GetCategories());
        }


    }
}