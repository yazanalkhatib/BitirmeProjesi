using BitirmeProjesiWeb.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BitirmeProjesiWeb.Models
{
    public class UserRole
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

        
        [DisplayName("اسم الوظيفة")]
        public string NameAr { get; set; }
        [DisplayName("Job Title")]
        public string NameEn { get; set; }
        [DisplayName("İş Pozisyonu")]
        [CustomRequired]
        public string NameTr { get; set; }

        public ICollection<User> Users { get; set; }
    }
}