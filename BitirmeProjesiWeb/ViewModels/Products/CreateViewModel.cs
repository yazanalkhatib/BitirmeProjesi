using BitirmeProjesiWeb.CustomAttributes;
using BitirmeProjesiWeb.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;

namespace BitirmeProjesiWeb.ViewModels.Products
{
    public class CreateViewModel
    {
        [DisplayName("التصنيف الرئيسي")]
        public int CategoryId { get; set; }
        [DisplayName("التصنيف الرئيسي")]
        public Category Category { get; set; }

        [CustomRequired]
        [DisplayName("اسم المشروع")]
        public string Name { get; set; }

        //Description
        
        [DisplayName("الوصف")]
        public string DescriptionAr { get; set; }
        [DisplayName("Description")]
        public string DescriptionEn { get; set; }
        [DisplayName("Açıklama")]
        [CustomRequired]
        public string DescriptionTr { get; set; }

        [DisplayName("صورة الغلاف")]
        public IFormFile CoverPhoto { get; set; }
        [DisplayName("صور الألبوم")]
        public List<IFormFile> Photos { get; set; }
    }
}