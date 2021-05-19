using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiWeb.CustomAttributes
{
    public class CustomRequiredStarAttribute : RequiredAttribute
    {
        public CustomRequiredStarAttribute()
        {
            ErrorMessage = "*";
        }
    }
}