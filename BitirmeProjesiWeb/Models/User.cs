using BitirmeProjesiWeb.CustomAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiWeb.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName("İş Pozisyonu	")]
        public int UserRoleId { get; set; }
        [DisplayName("İş Pozisyonu	")]
        public UserRole UserRole { get; set; }

        [CustomRequired]
        [DisplayName("Ad Soyad")]
        public string FullName { get; set; }

        [CustomRequired]
        [EmailAddress(ErrorMessage = "Bu Email uygun değildir")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [CustomRequired]
        [PasswordMinLengthValidator]
        [DisplayName("şifre")]
        public string Password { get; set; }

        [DisplayName("web sitesinde gösterme durumu")]
        public bool DisplayForClients { get; set; }
    }
}