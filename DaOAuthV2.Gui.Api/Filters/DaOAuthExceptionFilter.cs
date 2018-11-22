using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace DaOAuthV2.Gui.Api.Filters
{
    public class DaOAuthExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;

        public DaOAuthExceptionFilter(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is DaOAuthServiceException)
            {               
                _loggerFactory.CreateLogger<DaOAuthServiceException>().LogError(context.Exception, context.Exception.Message);
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                _loggerFactory.CreateLogger<Exception>().LogError(context.Exception, context.Exception.Message);
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.StatusCode = 500;
                }
            }

            context.Result = new JsonResult(new ErrorApiResultDto()
            {
                Message = context.Exception.Message,
                Details = _hostingEnvironment.IsDevelopment()?context.Exception.ToString():null
            });                      
        }
    }
}
