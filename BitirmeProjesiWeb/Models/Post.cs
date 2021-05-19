using BitirmeProjesiWeb.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BitirmeProjesiWeb.Models
{
    public class Post
    {
        public int Id { get; set; }

        //Foreign key for SubCategory
        [DisplayName("Alt Kategori Adı")]
        public int SubCategoryId { get; set; }
        [DisplayName("Alt Kategori Adı")]
        public SubCategory SubCategory { get; set; }

        //Title

        public string Title
        {
            get
            {
                string langCode = Thread.CurrentThread.CurrentUICulture.Name;

                return langCode switch
                {
                    "en" => TitleEn ?? TitleAr,
                    "tr" => TitleTr ?? TitleAr,
                    _ => TitleAr,
                };
            }
        }

        [DisplayName("العنوان")]
        public string TitleAr { get; set; }
        [DisplayName("Title")]
        public string TitleEn { get; set; }
        [DisplayName("Başlık")]
        [CustomRequired]
        public string TitleTr { get; set; }

        //Description

        public string Description
        {
            get
            {
                string langCode = Thread.CurrentThread.CurrentUICulture.Name;

                return langCode switch
                {
                    "en" => DescriptionEn ?? DescriptionAr,
                    "tr" => DescriptionTr ?? DescriptionAr,
                    _ => DescriptionAr,
                };
            }
        }

        [DisplayName("النص")]
        public string DescriptionAr { get; set; }
        [DisplayName("Article")]
        public string DescriptionEn { get; set; }
        [DisplayName("Metin")]
        [CustomRequired]
        public string DescriptionTr { get; set; }

        public string CoverPhotoPath { get; set; }
        public List<Photo> Photos { get; set; }
    }
}