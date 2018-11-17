using DaOAuthV2.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DaOAuthV2.Gui.Api.Filters
{
    public class DaOAuthExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public DaOAuthExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is DaOAuthServiceException)
                context.HttpContext.Response.StatusCode = 400;
            else
                context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(new ApiResult()
            {
                Message = context.Exception.Message,
                Details = _hostingEnvironment.IsDevelopment()?context.Exception.ToString():null
            });                      
        }

        private class ApiResult
        {
            public string Message { get; set; }
            public string Details { get; set; }
        }
    }
}
