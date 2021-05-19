using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiWeb.CustomAttributes
{
    public class CustomRequiredAttribute : RequiredAttribute
    {
        public CustomRequiredAttribute()
        {
            ErrorMessage = "Bu alan gereklidir";
        }
    }
}