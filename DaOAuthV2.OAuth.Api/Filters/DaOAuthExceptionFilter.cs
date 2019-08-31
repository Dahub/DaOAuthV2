using DaOAuthV2.Service;
using DaOAuthV2.Service.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace DaOAuthV2.OAuth.Api.Filters
{
    /// <summary>
    /// Custom exception filter
    /// Used to filter custom exception and return appropriate http response
    /// </summary>
    public class DaOAuthExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;

        public DaOAuthExceptionFilter(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Check the exception type, to specify the correct http code to the context
        /// </summary>
        /// <param name="context">http context in wich exception occur</param>
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
            else if (context.Exception is DaOauthUnauthorizeException)
            {
                _loggerFactory.CreateLogger<DaOauthUnauthorizeException>().LogError(context.Exception, context.Exception.Message);
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.StatusCode = 401;
                }
            }
            else if (context.Exception is DaOAuthRedirectException)
            {
                _loggerFactory.CreateLogger<DaOAuthRedirectException>().LogError(context.Exception, context.Exception.Message);
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.StatusCode = 302;
                    var url = ((DaOAuthRedirectException)context.Exception).RedirectUri;
                    context.HttpContext.Response.Redirect(url.AbsoluteUri);
                }
            }
            else if (context.Exception is DaOAuthTokenException)
            {
                _loggerFactory.CreateLogger<DaOAuthTokenException>().LogError(context.Exception, context.Exception.Message);
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.StatusCode = 400;
                    var json =  String.Empty;
                    var ex = (DaOAuthTokenException)context.Exception;
                    if(String.IsNullOrWhiteSpace(ex.State))
                    {
                        context.Result = new JsonResult(new
                        {
                            error = ex.Error,
                            error_description = ex.Description
                        });
                    }
                    else
                    {
                        context.Result = new JsonResult(new
                        {
                            error = ex.Error,
                            error_description = ex.Description,
                            state = ex.State
                        });
                    }
                    return;
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
                Details = _hostingEnvironment.IsDevelopment() ? context.Exception.ToString() : null
            });
        }
    }
}
