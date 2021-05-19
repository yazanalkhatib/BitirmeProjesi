using BitirmeProjesiWeb.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BitirmeProjesiWeb.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        //Foreign key for Category
        [DisplayName("Ana Kategori Adı")]
        public int CategoryId { get; set; }
        [DisplayName("Ana Kategori Adı")]
        public Category Category { get; set; }

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

        
        [DisplayName("اسم التصنيف الفرعي")]
        public string NameAr { get; set; }

        [DisplayName("Subcategory Name")]
        public string NameEn { get; set; }

        [DisplayName("Alt Kategori Adı")]
        [CustomRequired]
        public string NameTr { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}