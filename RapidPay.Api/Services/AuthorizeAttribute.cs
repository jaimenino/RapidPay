using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RapidPay.Application.Model;
using RapidPay.Application.Services;
using System;
using System.Linq;
using System.Net.Http.Headers;

//Code based on https://jasonwatmore.com/post/2021/12/20/net-6-basic-authentication-tutorial-with-example-api#authorize-attribute-cs

namespace RapidPay.Api.Services
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    { 
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isAuthorized = false;
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var authenticatedUser = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]); //(AuthenticatedUser)context.HttpContext.Items["AuthenticatedUser"];
            if (authenticatedUser != null)
            {
                isAuthorized = new AuthenticationService().ValidateTokenProperties(authenticatedUser.Parameter);
            }

            if (!isAuthorized)
            {
                // not logged in - return 401 unauthorized
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

                // set 'WWW-Authenticate' header to trigger login popup in browsers
                //context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"\", charset=\"UTF-8\"";
            }
        }
    }
}
