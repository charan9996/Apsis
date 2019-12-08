using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apsis.Models.Attributes
{
    /// <summary>
    /// Generic exception attribute
    /// </summary>
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Overide for OnException
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            Response.Response response = new Response.Response
            {
                Message = "An error has occured."
            };
            context.HttpContext.Response.StatusCode = 500;
            //context.Result = new JsonResult(response);
            base.OnException(context);
        }
    }
}
