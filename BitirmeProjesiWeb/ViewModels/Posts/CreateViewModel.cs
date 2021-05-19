using BitirmeProjesiWeb.CustomAttributes;
using BitirmeProjesiWeb.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;

namespace BitirmeProjesiWeb.ViewModels.Posts
{
    public class CreateViewModel
    {
        //Foreign key for SubCategory
        [DisplayName("التصنيف الفرعي")]
        public int SubCategoryId { get; set; }
        [DisplayName("التصنيف الفرعي")]
        public SubCategory SubCategory { get; set; }

        //Title
        
        [DisplayName("العنوان")]
        public string TitleAr { get; set; }
        [DisplayName("Title")]

        public string TitleEn { get; set; }
        [DisplayName("Başlık")]
        [CustomRequired]
        public string TitleTr { get; set; }

        //Description
        
        [DisplayName("النص")]
        public string DescriptionAr { get; set; }
        [DisplayName("Article")]
        public string DescriptionEn { get; set; }
        [DisplayName("Yazı")]
        [CustomRequired]
        public string DescriptionTr { get; set; }

        [DisplayName("صورة الغلاف")]
        public IFormFile CoverPhoto { get; set; }

        [DisplayName("الصور")]
        public List<IFormFile> Photos { get; set; }
    }
}