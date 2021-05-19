using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace BitirmeProjesiWeb.Utilities
{
    public class Security : ISecurity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Security(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserFullName => Authenticate() ? _httpContextAccessor.HttpContext.Session.GetString("userFullName") : null;
            
        public bool Authenticate()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("userId") != null;
            //return true;
        }

        public bool AuthorizeAdmin()
        {
            if (Authenticate())
            {
                return _httpContextAccessor.HttpContext.Session.GetString("userRoleId").Equals("1");
            }

            return false;
            //return true;
        }
    }
}