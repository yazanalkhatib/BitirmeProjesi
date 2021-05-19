using BitirmeProjesiWeb.CustomAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiWeb.ViewModels
{
    public class LoginViewModel
    {
        [CustomRequired]
        [EmailAddress(ErrorMessage = "email doğru değildir")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [CustomRequired]
        [PasswordMinLengthValidator]
        [DisplayName("şifre")]
        public string Password { get; set; }
    }
}