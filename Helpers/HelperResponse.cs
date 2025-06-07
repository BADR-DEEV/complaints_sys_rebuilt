using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models;
using Microsoft.AspNetCore.Mvc;

namespace complaints_back.Helpers
{
    public class Helpers<T> : ControllerBase
    {
        public ServiceResponse<T> response { get; set; } = new ServiceResponse<T>();
        public ActionResult<ServiceResponse<T>> HandleResponse(ServiceResponse<T> response)
        {
            var unifiedResponse = new ServiceResponse<T>
            {
                Success = response.Success,
                Message = response.Message,
                StatusCode = response.StatusCode,
                Data = response.Data
            };

            switch (response.StatusCode)
            {
                case 500:
                    return StatusCode(500, unifiedResponse);
                case 400:
                    return BadRequest(unifiedResponse);
                case 200:
                    return Ok(unifiedResponse);
                case 201:
                    return Created("", unifiedResponse);
                case 404:
                    return NotFound(unifiedResponse);
                default:
                    return BadRequest(unifiedResponse);
            }
        }
    }
}