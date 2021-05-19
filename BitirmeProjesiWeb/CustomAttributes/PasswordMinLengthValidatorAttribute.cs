using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiWeb.CustomAttributes
{
    public class PasswordMinLengthValidatorAttribute : MinLengthAttribute
    {
        public PasswordMinLengthValidatorAttribute() : base(8)
        {
            ErrorMessage = "en az 8 haneli şifre olmalı";
        }
    }
}