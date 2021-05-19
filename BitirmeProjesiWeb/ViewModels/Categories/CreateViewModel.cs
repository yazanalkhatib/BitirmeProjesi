using BitirmeProjesiWeb.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace BitirmeProjesiWeb.ViewModels.Categories
{
    public class CreateViewModel
    {
        
        [DisplayName("اسم التصنيف")]
        public string NameAr { get; set; }

        [DisplayName("Category Name")]
        public string NameEn { get; set; }
        [CustomRequired]
        [DisplayName("Kategori Adı")]
        public string NameTr { get; set; }

        [DisplayName("صورة الغلاف")]
        public IFormFile Photo { get; set; }
    }
}