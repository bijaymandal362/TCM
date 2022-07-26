using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Constant.Authorization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Attributes
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        private  readonly string actionName = string.Empty;
        public PermissionAttribute(string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(actionName, claimValue) };
        }
        public class ClaimRequirementFilter : IAsyncActionFilter
        {
            private readonly Claim _claim;
            public ClaimRequirementFilter(Claim claim)
            {
                _claim = claim;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                Claim permissionClaim = context.HttpContext.User.FindFirst(TokenKey.Permission);
             
                if (permissionClaim != null && !string.IsNullOrEmpty(permissionClaim.Value))
                {
                    try
                    {
                        var userPermissions = JsonConvert.DeserializeObject<List<string>>(permissionClaim.Value);
                        if (userPermissions.Contains(_claim.Value))
                        //|| PermissionCheckControllers.ValidateControllerActionMethod(actionDescriptor.ControllerName, actionDescriptor.ActionName))
                        {
                            await next();
                            return;
                        }
                    }
                    catch
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.HttpContext.Response.Redirect("/error/403");
                    }
                }

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
          
            }
        }
    }

}
