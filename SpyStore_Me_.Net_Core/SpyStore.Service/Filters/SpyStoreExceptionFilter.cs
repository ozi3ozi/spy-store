using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SpyStore.Dal.Exceptions;

//Filters are an extremely powerful feature of ASP.NET Core. In this chapter, we are only examining exception filters, 
//but there are many more that can be created that can save significant time when building ASP.NET Core applications. 
//For the full information on filters, refer to the documentation here: 
//https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters.
namespace SpyStore.Service.Filters
{
    public class SpyStoreExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        //Constructor is called is Startup class to set the filters globally
        public SpyStoreExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //ExceptionContext parameter provides access to the ActionContext as well as the exception that was thrown.
        //The code in the OnException event handler checks the type of exception through (custom or system) and builds 
        //an appropriate response. If the environment is Development, include the stack trace in the message. The flow is 
        //very straightforward: Determine the type of exception that occurred, build a dynamic object to contain the 
        //values to be sent to the calling request, and return an appropriate IActionResult. Each of the 
        //BadRequestObjectResult and ObjectResult convert the anonymous objects into JSON as part of the HTTP response.
        public override void OnException(ExceptionContext context)
        {
            bool isDevelopment = _hostingEnvironment.IsDevelopment();
            var ex = context.Exception;
            string stackTrace = (isDevelopment) ? context.Exception.StackTrace : string.Empty;
            string message = ex.Message;
            string error = string.Empty;
            IActionResult actionResult;
            switch (ex)
            {
                case SpyStoreInvalidQuantityException iqe:
                    //returns a 400
                    error = "Invalid quantity request.";
                    actionResult = new BadRequestObjectResult(new
                        { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                case DbUpdateConcurrencyException ce:
                    //returns a 400
                    error = "Concurrency exception.";
                    actionResult = new BadRequestObjectResult(new
                        { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                case SpyStoreInvalidProductException ipe:
                    //returns a 400
                    error = "Invalid product id.";
                    actionResult = new BadRequestObjectResult(new
                        { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                case SpyStoreInvalidCustomerException ice:
                    //returns a 400
                    error = "Invalid customer id.";
                    actionResult = new BadRequestObjectResult(new
                        { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                default:
                    error = "General error.";
                    actionResult = new ObjectResult(new
                        { Error = error, Message = message, StackTrace = stackTrace })
                        { StatusCode = 500 };
                    break;
            }
            //If you want the exception filter to swallow the exception and set the response to a 200 
            //(e.g., to log the error but not return it to the client), add the following line before 
            //setting the Result
            //context.ExceptionHandled = true;
            context.Result = actionResult;
        }
    }
}
