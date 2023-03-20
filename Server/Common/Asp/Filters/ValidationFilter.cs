using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Types.Dtos;

namespace CommonServer.Asp.Filters;

public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new ErrorDto(
                $"validation errors: {string.Join("; ", context.ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage).ToList())}",
                HttpStatusCode.BadRequest
            ));
        }
    }
}
