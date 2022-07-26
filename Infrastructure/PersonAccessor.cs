using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class PersonAccessor : IPersonAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PersonAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }

        public int GetPersonId()
        {
            if (IsPersonAuthenticated())
            {
                var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                return Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
            }
            else
                throw new Exception("Unauthorized");
            
        }

        public bool IsPersonAuthenticated()
        {
            if (_httpContextAccessor.HttpContext == null) return false;
            else return true;
        }
    }
}
