using BitirmeProjesiWeb.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BitirmeProjesiWeb.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Ana kategori")]
        public int CategoryId { get; set; }
        [DisplayName("Ana kategori")]
        public Category Category { get; set; }

        [DisplayName("projenin adı")]
        public string Name { get; set; }

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

        [DisplayName("الوصف")]
        public string DescriptionAr { get; set; }
        [DisplayName("Description")]
        public string DescriptionEn { get; set; }
        [DisplayName("Açıklama")]

        [CustomRequired]
        public string DescriptionTr { get; set; }

        public string CoverPhotoPath { get; set; }
        public List<ProductPhoto> Photos { get; set; }
    }
}