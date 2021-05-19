using BitirmeProjesiWeb.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BitirmeProjesiWeb.Models
{
    public class Category
    {
        
        public int Id { get; set; }

        public string Name 
        { 
            get
            {
                string langCode = Thread.CurrentThread.CurrentUICulture.Name;

                return langCode switch
                {
                    "en" => NameEn ?? NameAr,
                    "tr" => NameTr ?? NameAr,
                    _ => NameAr
                };
            }
        }


        [DisplayName("اسم التصنيف")]
        public string NameAr { get; set; }

        [DisplayName("Category Name")]
        public string NameEn { get; set; }
        
        [DisplayName("Kategori Adı")]

        [CustomRequired]
        public string NameTr { get; set; }

        public string CoverPhotoPath { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}