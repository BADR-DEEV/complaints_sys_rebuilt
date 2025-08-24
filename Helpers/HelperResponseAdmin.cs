using complaints_back.models;
using Microsoft.AspNetCore.Mvc;

public class HelpersAdmin<T> : ControllerBase
{
    public ActionResult<ServiceResponseAdmin<T>> HandleResponse(ServiceResponseAdmin<T> response)
    {
        var unifiedResponse = new ServiceResponseAdmin<T>
        {
            Success = response.Success,
            Message = response.Message,
            StatusCode = response.StatusCode,
            Data = response.Data,
            PageNumber = response.PageNumber,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount
        };

        return response.StatusCode switch
        {
            500 => StatusCode(500, unifiedResponse),
            403 => StatusCode(403, unifiedResponse),
            400 => BadRequest(unifiedResponse),
            200 => Ok(unifiedResponse),
            201 => Created("", unifiedResponse),
            404 => NotFound(unifiedResponse),
            _ => BadRequest(unifiedResponse)
        };
    }
}
