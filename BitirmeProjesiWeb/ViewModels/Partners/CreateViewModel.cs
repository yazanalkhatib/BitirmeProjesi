using BitirmeProjesiWeb.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace BitirmeProjesiWeb.ViewModels.Partners
{
    public class CreateViewModel
    {
        [CustomRequired]
        [DisplayName("اسم الشركة")]
        public string Name { get; set; }
        [CustomRequired]
        [DisplayName("عنوان الموقع الالكتروني")]
        public string WebsiteLink { get; set; }
        [DisplayName("صورة الشعار")]
        public IFormFile Logo { get; set; }
    }
}